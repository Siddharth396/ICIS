function waitfor {
    url=$1

    gotResponse=false
    maxRetries=20
    currentTry=1

    echo "Waiting for $url to start"

    while [[ "$gotResponse" != "true" && currentTry -le maxRetries ]]
    do
	    echo "Attempt $currentTry of $maxRetries"
	    response=$(curl -k --write-out %{http_code} --silent --output /dev/null $url)
        if [ "$response" == "200" ]
        then
          echo "Got response successfully"
          gotResponse=true
        else
          echo "No response from $url. Got:"
          echo $response
          sleep 5
        fi
        currentTry=$((currentTry+1))
    done
}