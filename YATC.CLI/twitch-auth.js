window.onload = function() {
    const params = new URLSearchParams(window.location.hash.substring(1));
    const accessToken = params.get('access_token');

    if (accessToken) {
        const tokenUrl = `http://localhost:1234/?access_token=${encodeURIComponent(accessToken)}`;

        fetch(tokenUrl, { method: 'GET', mode: 'no-cors' })
            .then(response => {
                if (response.ok) {
                    document.body.innerHTML = "<h1>Authorization successful! You can go back to YATC now.</h1>";
                } else {
                    console.error('Failed to send access token to the server');
                    document.body.innerHTML = "<h1>Error: Failed to send access token to the server. Please try again.</h1>";
                }
            });
    } else {
        console.error('Access token not found in URL');
        document.body.innerHTML = "<h1>Error: Access token not found. Please try again.</h1>";
    }
};
