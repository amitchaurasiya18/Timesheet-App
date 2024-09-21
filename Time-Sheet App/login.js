const loginURL = "https://localhost:7206/api/User/Login";

document.querySelector('#login-form').addEventListener('submit', async function(event) {
    event.preventDefault();

    const username = document.querySelector('#user-username').value;
    const password = document.querySelector('#user-password').value;

    const response = await fetch(loginURL, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            Username: username,
            Password: password
        })
    });

    if (response.ok) {
        const token = await response.text();
        sessionStorage.setItem('jwtToken', token);
        if (token != "Not a Valid User") {
            window.location.href = '/timesheet.html';
        } else {
            alert("User Not Found!!")
        }
    } else {
        alert('Invalid username or password');
    }
});