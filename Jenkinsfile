@Library('global-shared-library') _
def message = "";

pipeline {
	agent any

	environment {
		BuildConfiguration = "Debug"
		ImageName = "harbor.rpgp.cc/tgs/payment-help2pay"
	}
	stages {
		stage('Prepare'){
			steps {
				sh "echo 'Branch: $GIT_BRANCH'";
                sh "printenv";
				script {
					switch(env.GIT_BRANCH)
                    {
                        case ['main', 'origin/main']:
                            env.kube_ns = "prod";
                            env.k8s_config = 'stag-k8s-config';
                            env.node_selector = '{ "dedicated": "prod" }';
                            env.dedicated = '{"effect":"NoSchedule","key": "dedicated", "operator": "Equal", "value": "prod"}';
                            break;
                        case ['release', 'origin/release']:
                            env.kube_ns = "staging";
                            env.k8s_config = 'stag-k8s-config';
                            env.node_selector = '{ "dedicated": "nonprod" }';
                            env.dedicated = '{"effect":"NoSchedule","key": "dedicated", "operator": "Equal", "value": "noprod"}';
                            break;
                        default:
                            env.kube_ns = "dev";
                            env.k8s_config = 'k8s-kube-config';
                            env.node_selector = '{}';
                            env.dedicated = ''; 
                            break;
                    }
				}
				configFileProvider([
                    configFile(
                        fileId: 'tgs.' + env.kube_ns , 
                        variable: 'tgsconfig')]) {
                    script {
                        def envItems = readProperties file: "$tgsconfig";

                        envItems.each { key, value -> 
                            env."tgs_${key}" = value;
                        }

                        env.TAG = sh(script: "echo `date +%Y%m%d%H%M`", returnStdout: true).trim() + '-' + env.kube_ns
                        message += "\nBranch: ${env.GIT_BRANCH}" 
                        message += "\nTag: ${env.TAG}"
                        message += "\nEnv: ${env.kube_ns}"
                        env.commit_url = "${env.GIT_URL}".replace("git@gitlab.com:","https://gitlab.com/")
                            .replace(".git","/-/commit/") + "${env.GIT_COMMIT}"
                        message += "\n Commit: ${env.commit_url}"
                    }
				}
                sh "echo tag: $TAG"
			}
		}

		stage('Build') {
			steps {
				sh 'dotnet build -c $BuildConfiguration'
			}
		}

		stage('Test') {
			steps {
				sh 'dotnet test -c $BuildConfiguration --logger trx --no-build'
			}
		}
		
		stage('Push packages') {
			when {
                expression {
                    return env.kube_ns == 'dev';
                }
            }

			steps {
				sh 'dotnet pack -c $BuildConfiguration "Tgs.Payment.Help2Pay.Client/Tgs.Payment.Help2Pay.Client.csproj" --no-build'
				sh 'dotnet nuget push "Tgs.Payment.Help2Pay.Client/bin/$BuildConfiguration/*.nupkg" -s "raypro-baget" -k admin --skip-duplicate'
			}
		}

		stage('Build Docker Image') {
			steps {
				sh 'dotnet build "Tgs.Payment.Help2Pay/Tgs.Payment.Help2Pay.csproj" -c $BuildConfiguration -t:ContainerBuild -p:DockerfileTag="$ImageName:${TAG}" --no-restore '
			}
		}

		stage('Push Docker Image') {
			steps {
				sh 'sudo docker push $ImageName:${TAG}'
			}
		}

		stage('Deploy') {
            when {
                expression {
                    return env.kube_ns == 'prod';
                }
            }

            steps {
                script {
                    input message: 'Ready to go?', ok: 'yes';
                }
                
                sh "echo start deploy"
            }
        }

		stage('Deploy to k8s') {			
			steps {
                script {
                    withKubeConfig([credentialsId: env.k8s_config]) {

                        def resources = getResources(env.k8s_config, env.kube_ns, "payment-help2pay")
                        println resources

                        withCredentials([string(credentialsId: "tgs-${kube_ns}-connectionstring", variable: "tgs_tgs_conn")]) {
                            sh """
                                cat << EOF | yq e -P | helm template deployment/k8s/help2pay \
                                    -n ${kube_ns} \
                                    --set image.tag=${TAG} \
                                    --set connectionString.redis=$tgs_redis \
                                    --set paymentDepositApiUrl=$tgs_paymentDepositApiUrl_help2pay \
                                    --set paymentWithdrawApiUrl=$tgs_paymentWithdrawApiUrl_help2pay \
                                    --set paymentQueryApiUrl=$tgs_paymentQueryApiUrl_help2pay \
                                    --set nats=$tgs_nats \
                                    --set callbackBaseUrl=$tgs_paymentCallbackBaseUrl_help2pay \
                                    -f -
                                {
                                    "nodeSelector": $node_selector,
                                    "tolerations": [$dedicated],
                                    "resources": $resources
                                }
                            """
                            sh """
                                cat << EOF | yq e -P | helm template deployment/k8s/help2pay \
                                    -n ${kube_ns} \
                                    --set image.tag=${TAG} \
                                    --set connectionString.redis=$tgs_redis \
                                    --set paymentDepositApiUrl=$tgs_paymentDepositApiUrl_help2pay \
                                    --set paymentWithdrawApiUrl=$tgs_paymentWithdrawApiUrl_help2pay \
                                    --set paymentQueryApiUrl=$tgs_paymentQueryApiUrl_help2pay \
                                    --set nats=$tgs_nats \
                                    --set callbackBaseUrl=$tgs_paymentCallbackBaseUrl_help2pay \
                                    -f - \
                                | kubectl -n ${kube_ns} apply -f -
                                {
                                    "nodeSelector": $node_selector,
                                    "tolerations": [$dedicated],
                                    "resources": $resources
                                }
                            """
                        }
                    }
                }
			}
		}

		stage('Add Tag on Prod') {
            when {
                expression {
                    return env.kube_ns == 'prod';
                }
            }		
            steps {
                sshagent(['ssh-jenkins']) {
                    sh "git tag -a ${TAG} -m ${TAG}"
                    sh "git push ${GIT_URL} ${TAG} HEAD:${GIT_BRANCH}"
                }
            }
        }		
	}

	post {
		always {
            step([$class: 'MSTestPublisher', testResultsFile:"**/*.trx", failOnError: false, keepLongStdio: true])
        }
        success {
             sendTelegram("Pipeline: ${currentBuild.fullDisplayName} OK! ${message} \n${currentBuild.getBuildCauses()[0].shortDescription}\n")
        }
        failure {
            sendTelegram("<b>Failed Pipeline:</b> ${currentBuild.fullDisplayName} Something is wrong with ${env.BUILD_URL}/console\n${currentBuild.getBuildCauses()[0].shortDescription}\n")
        }		
	}
}
