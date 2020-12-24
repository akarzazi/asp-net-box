$pfx_cert = get-content .\localhost.pfx -AsByteStream
[System.Convert]::ToBase64String($pfx_cert)