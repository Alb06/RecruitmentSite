import os
import requests

GITLAB_TOKEN = os.environ['GITLAB_TOKEN']
GITHUB_TOKEN = os.environ['GITHUB_TOKEN']
GITLAB_PROJECT_ID = os.environ['GITLAB_PROJECT_ID']
GITHUB_REPO = os.environ['GITHUB_REPO']

# Get issues from GitLab
issues = requests.get(
    f'https://gitlab.com/api/v4/projects/{GITLAB_PROJECT_ID}/issues',
    headers={'PRIVATE-TOKEN': GITLAB_TOKEN}
).json()

# Create issues in GitHub
for issue in issues:
    data = {
        'title': issue['title'],
        'body': f"Imported from GITLAB:\n\n{issue['description']}",
        'labels': ['imported-from-gitlab']
    }
    requests.post(
        f'https://api.github.com/repos/{GITHUB_REPO}/issues',
        headers={'Authorization': f'token {GITHUB_TOKEN}'},
        json=data
    )