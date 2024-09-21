const apiUrl = "https://localhost:7206/api/Timesheet";
const activityElement = document.getElementById("timsheets");
const editActivityElement = document.getElementById("edit-timsheets");

// Fetch JWT Token
const getJwtToken = () => sessionStorage.getItem("jwtToken");

// Generic Fetch Function
const apiFetch = async(endpoint, options = {}) => {
    const response = await fetch(endpoint, {
        ...options,
        headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${getJwtToken()}`,
            ...options.headers
        }
    });

    if (response.status === 401) {
        alert("Session Timeout");
        sessionStorage.clear();
        window.location.href = "login.html";
    }

    return response.ok ? response.json() : null;
};

// Fetch User ID
const fetchUserId = async() => {
    const { userId } = await apiFetch("https://localhost:7206/api/User/GetUser");
    return userId;
};

// Handle Adding/Editing Activities
const renderActivity = (container, data = {}) => {
        const { project = "", subproject = "", batch = "", activity = "", hoursNeeded = "" } = data;
        container.insertAdjacentHTML("beforeend", `
    <div class="card">
        <div class="card-body">
            <div class="row">
                ${["project", "subproject", "batch"].map(field => `
                    <div class="col-md-4">
                        <div class="form-group">
                            <label for="${field}">${field.charAt(0).toUpperCase() + field.slice(1)}</label>
                            <input type="text" class="form-control ${field}-select" placeholder="${field.charAt(0).toUpperCase() + field.slice(1)}" value="${data[field] || ''}">
                        </div>
                    </div>`).join('')}
            </div>
            <div class="row">
                <div class="col-md-6">
                    <div class="form-group">
                        <label for="activity">Activity</label>
                        <input type="text" class="form-control activity-select" placeholder="Activity" value="${activity || ''}">
                    </div>
                </div>
                <div class="col-md-3">
                    <div class="form-group">
                        <label for="hours">Hours Needed</label>
                        <input type="number" class="form-control hours-select" placeholder="Hours Needed" value="${hoursNeeded || ''}">
                    </div>
                </div>
                <div class="col-md-3">
                    <button type="button" class="btn btn-danger btn-sm remove-activity">Remove</button>
                </div>
            </div>
        </div>
    </div>`);
};

let count = 1;

// Add Timesheet Activity
const addActivity = () => {
    console.log(count++);
    
    const existingActivities = Array.from(activityElement.children).map(card => ({
        project: card.querySelector(".project-select").value,
        subproject: card.querySelector(".subproject-select").value,
        batch: card.querySelector(".batch-select").value,
        activity: card.querySelector(".activity-select").value,
    }));

    const newActivity = {}; // Gather your new activity data here

    // Check if the new activity already exists
    if (!existingActivities.some(activity => 
        activity.project === newActivity.project &&
        activity.subproject === newActivity.subproject &&
        activity.batch === newActivity.batch &&
        activity.activity === newActivity.activity)) {
        renderActivity(activityElement, newActivity);
    } else {
        alert("This activity already exists.");
    }
};


// Save Timesheet Data
const onSave = async () => {
    try {
        const userId = await fetchUserId();
        if (!userId) return;

        const data = {
            UserId: userId,
            Date: document.getElementById("date").value,
            OnLeave: document.getElementById("onLeave").value === "Yes",
            Activities: Array.from(document.querySelectorAll(".card")).map(card => ({
                Project: card.querySelector(".project-select").value,
                SubProject: card.querySelector(".subproject-select").value,
                Batch: card.querySelector(".batch-select").value,
                ActivityDescription: card.querySelector(".activity-select").value,
                HoursNeeded: parseFloat(card.querySelector(".hours-select").value)
            }))
        };

        // Ensure no duplicate entries before posting
        const uniqueActivities = Array.from(new Set(data.Activities.map(JSON.stringify)))
            .map(JSON.parse);

        if (await apiFetch(apiUrl, { method: "POST", body: JSON.stringify({ ...data, Activities: uniqueActivities }) })) {
            alert("Timesheet added successfully!");
            window.location.reload();
        }
    } catch (error) {
        console.error("Error saving timesheet:", error);
    }
};


// Preview Timesheet
const preview = () => {
    const tableBody = document.querySelector("#preview-body");
    tableBody.innerHTML = ""; // Clear previous entries

    let totalHours = 0;

    Array.from(document.querySelectorAll(".card")).forEach((card, index) => {
        const project = card.querySelector(".project-select").value;
        const subProject = card.querySelector(".subproject-select").value;
        const batch = card.querySelector(".batch-select").value;
        const activity = card.querySelector(".activity-select").value;
        const hoursNeeded = parseFloat(card.querySelector(".hours-select").value) || 0; // Ensure a number

        totalHours += hoursNeeded;

        tableBody.insertAdjacentHTML("beforeend", `
            <tr>
                <td>${index + 1}</td>
                <td>${document.getElementById("date").value}</td>
                <td>${document.getElementById("onLeave").value}</td>
                <td>${project}</td>
                <td>${subProject}</td>
                <td>${batch}</td>
                <td>${activity}</td>
                <td>${hoursNeeded}</td>
                <td>${totalHours}</td>
            </tr>`);
    });
};

// Cancel Timesheet Form
const cancel = () => {
    document.getElementById("activity-form").reset();
    document.querySelectorAll("#timsheets .card").forEach(card => card.remove());
    showContent("default-content");
};

// Show Content Section
const showContent = (contentId) => {
    document.querySelectorAll(".content").forEach(section => section.style.display = "none");
    document.getElementById(contentId).style.display = "block";
};

// Fetch and Render Timesheets
const fetchAndRenderTimesheets = async (containerId) => {
    const timesheets = await apiFetch(apiUrl);
    const tableBody = document.querySelector(containerId);
    tableBody.innerHTML = "";

    timesheets.forEach(ts => {
        tableBody.insertAdjacentHTML("beforeend", `
            <tr>
                <td>${ts.timesheetId}</td>
                <td>${ts.date}</td>
                <td>${ts.onLeave ? "Yes" : "No"}</td>
                <td>${ts.activities.map(a => a.project).join(", ")}</td>
                <td>${ts.activities.map(a => a.subProject).join(", ")}</td>
                <td>${ts.activities.map(a => a.batch).join(", ")}</td>
                <td>${ts.activities.map(a => a.activityDescription).join(", ")}</td>
                <td>${ts.activities.map(a => a.hoursNeeded).join(", ")}</td>
            </tr>`);
    });
};

// Fetch Timesheet for Editing
const fetchTimesheetForEdit = async (id) => {
    const timesheet = await apiFetch(`${apiUrl}/${id}`);
    if (!timesheet) return alert("Timesheet not found.");

    document.getElementById("edit-date").value = timesheet.date;
    document.getElementById("edit-onLeave").value = timesheet.onLeave ? "Yes" : "No";
    editActivityElement.innerHTML = "";

    timesheet.activities.forEach(activity => renderActivity(editActivityElement, activity));

    document.getElementById("edit-timesheet-form").style.display = "block";
};

// Save Edited Timesheet Data
const onSaveEdit = async () => {
    try {
        const id = document.getElementById("edit-id").value;
        const data = {
            TimesheetId: id,
            Date: document.getElementById("edit-date").value,
            OnLeave: document.getElementById("edit-onLeave").value === "Yes",
            Activities: Array.from(document.querySelectorAll("#edit-timsheets .card")).map(card => ({
                Project: card.querySelector(".project-select").value,
                SubProject: card.querySelector(".subproject-select").value,
                Batch: card.querySelector(".batch-select").value,
                ActivityDescription: card.querySelector(".activity-select").value,
                HoursNeeded: parseFloat(card.querySelector(".hours-select").value)
            }))
        };

        if (await apiFetch(`${apiUrl}/${id}`, { method: "PUT", body: JSON.stringify(data) })) {
            alert("Timesheet updated successfully!");
            hideEditForm();
            window.location.reload();
            // fetchAndRenderTimesheets("#timesheet-list"); // Refresh the timesheet list
        }
    } catch (error) {
        console.error("Error updating timesheet:", error);
    }
};

// Delete Timesheet Data
const deleteTimesheet = async (id) => {
    if (await apiFetch(`${apiUrl}/${id}`, { method: "DELETE" })) {
        alert("Timesheet deleted successfully!");
        fetchAndRenderTimesheets("#timesheet-list"); // Refresh the timesheet list
        showContent("default-content"); // Go back to the default content
    }
};

// Hide Edit Form
const hideEditForm = () => {
    document.getElementById("edit-timesheet-form").style.display = "none";
    document.getElementById("edit-id").value = ""; // Clear the ID input
};

// Event Listeners
document.getElementById("add-timesheets").addEventListener("click", () => {
    showContent("add-timesheet-content");
    addActivity();
});

document.getElementById("edit-timesheets").addEventListener("click", () => {
    showContent("edit-timesheet-content");
    fetchAndRenderTimesheets("#timesheet-list-edit");
});

document.getElementById("get-timesheets").addEventListener("click", () => {
    showContent("get-timesheet-content");
    fetchAndRenderTimesheets("#timesheet-list");
});

document.getElementById("delete-timesheets").addEventListener("click", () => showContent("delete-timesheet-content"));

document.getElementById("confirm-delete").addEventListener("click", () => {
    const deleteId = document.getElementById("delete-id").value;
    if (deleteId) deleteTimesheet(deleteId);
    else alert("Please enter a valid Timesheet ID.");
});

document.getElementById("cancel-delete").addEventListener("click", () => showContent("default-content"));

// document.getElementById("activity-button").addEventListener("click", addActivity);
document.getElementById("edit-activity-button").addEventListener("click", () => renderActivity(editActivityElement, {}));

document.addEventListener("click", (e) => {
    if (e.target.classList.contains("remove-activity")) e.target.closest(".card").remove();
});

document.getElementById("fetch-timesheet").addEventListener("click", async () => {
    const id = document.getElementById("edit-id").value;
    if (id) await fetchTimesheetForEdit(id);
    else alert("Please enter a valid Timesheet ID.");
});

document.getElementById("save-edited-timesheet").addEventListener("click", onSaveEdit);

// Add event listener for the preview button
document.getElementById("activity-button").addEventListener("click", addActivity);
document.getElementById("preview-button").addEventListener("click", preview);