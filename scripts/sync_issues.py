import os
import requests

GITLAB_TOKEN = os.environ['GITLAB_TOKEN']
GITHUB_TOKEN = os.environ['GITHUB_TOKEN']
GITLAB_PROJECT_ID = os.environ['GITLAB_PROJECT_ID']
GITHUB_REPO = os.environ['GITHUB_REPO']

headers_gitlab = {'PRIVATE-TOKEN': GITLAB_TOKEN}
headers_github = {'Authorization': f'token {GITHUB_TOKEN}'}

# Fetch GitLab issues
gitlab_issues = requests.get(
    f'https://gitlab.com/api/v4/projects/{GITLAB_PROJECT_ID}/issues',
    headers=headers_gitlab
).json()

# Fetch existing GitHub issues
github_issues = []
page = 1
while True:
    response = requests.get(
        f'https://api.github.com/repos/{GITHUB_REPO}/issues',
        headers=headers_github,
        params={'state': 'all', 'per_page': 100, 'page': page}
    )
    data = response.json()
    if not data:
        break
    github_issues.extend(data)
    page += 1

existing_titles = {issue['title'] for issue in github_issues}

# Create GitLab issues on GitHub if they don't already exist
for issue in gitlab_issues:
    if issue['title'] in existing_titles:
        print(f"Issue already exists, skipping: {issue['title']}")
        continue

    payload = {
        'title': issue['title'],
        'body': f"Imported from GitLab:\n\n{issue['description']}",
        'labels': ['imported-from-gitlab']
    }
    response = requests.post(
        f'https://api.github.com/repos/{GITHUB_REPO}/issues',
        headers=headers_github,
        json=payload
    )
    if response.status_code == 201:
        print(f"Issue created: {issue['title']}")
    else:
        print(f"Failed to create issue: {issue['title']} → {response.status_code} → {response.text}")

