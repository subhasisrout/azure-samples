import azure.functions as func
import logging
from azure.identity import ManagedIdentityCredential
import requests

app = func.FunctionApp(http_auth_level=func.AuthLevel.ANONYMOUS)

@app.route(route="http_trigger")
def http_trigger(req: func.HttpRequest) -> func.HttpResponse:
    logging.info('Python HTTP trigger function processed a request.')

    name = req.params.get('name')
    if not name:
        try:
            req_body = req.get_json()
        except ValueError:
            pass
        else:
            name = req_body.get('name')

    if name:
        return func.HttpResponse(f"Hello, {name}. This HTTP triggered function executed successfully.")
    else:
        return func.HttpResponse(
             "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response.",
             status_code=200
        )
    
@app.route(route="http_trigger_call_secure_api")
def http_trigger_call_secure_api(req: func.HttpRequest) -> func.HttpResponse:
    logging.info('Python HTTP trigger function for secure API call processed a request.')
    
    try:
        # Use ManagedIdentityCredential to get an access token with managed identity
        credential = ManagedIdentityCredential()
        scope = "api://c7c788a5-f95b-41d1-b677-ca2116fcf073/.default"
        token = credential.get_token(scope)
        
        # Prepare headers with the access token
        headers = {
            'Authorization': f'Bearer {token.token}',
            'Content-Type': 'application/json'
        }
        
        # Make the API call
        api_endpoint = "https://surout-fadvgwcpa5eefkfr.canadacentral-01.azurewebsites.net/weatherforecast"
        response = requests.get(api_endpoint, headers=headers, timeout=30)
        
        # Return the raw response
        return func.HttpResponse(
            body=response.text,
            status_code=response.status_code,
            mimetype="application/json" if response.headers.get('content-type', '').startswith('application/json') else "text/plain"
        )
        
    except Exception as e:
        logging.error(f"Error calling secure API: {str(e)}")
        return func.HttpResponse(
            f"Error calling secure API: {str(e)}",
            status_code=500
        )
