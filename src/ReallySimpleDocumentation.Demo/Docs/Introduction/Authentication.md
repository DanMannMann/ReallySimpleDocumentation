The API is secured using OAuth2. Interactive (username & password) login is supported on this documentation site for testing purposes, but for the final integration you will want to use the client credentials which have been supplied to your organisation by FollowApp Care.

The client credentials must be used to obtain a time-limited access token from the auth server, which for this environment is located at {{authUrl}}. The FollowApp Care authentication server supports requesting an API token by including the client credentials as URL-encoded form fields in a GET request:

* URL: {{authUrl}}connect/token
* Headers: 
    * **Content-Type**: `application/x-www-form-urlencoded`
* Form Fields:
    * **client_id**: `[your client id]`
	* **client_secret**: `[your client secret]`
	* **grant_type**: `client_credentials`
	* **scope**: `recalls-api`

The <a target="_blank" href="https://www.oauth.com/oauth2-servers/access-tokens/client-credentials/">OAuth2 documentation</a> describes using client credentials in more detail.

You should obtain a new access token each time your integration code runs. The token remains valid for 1 hour so should only need to be retrieved once per run, even if the run makes multiple calls to the FollowApp Care API.

Once you have retrieved an access token, it should be attached to every API request as a header named Authorization, with value `Bearer [your token]`, for example:
- key: `Authorization`
- value: `Bearer abCdEfG12345ghgh4564654`