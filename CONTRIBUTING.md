# Contributing Guidelines

## Branch Naming Convention

Branches must follow the format:

<issue-number>-<story-or-task-id>-<short-description>

Examples:

- 5-t-18-1-user-status
- 4-us-17-user-management

## Development Workflow

1. Create or select a GitHub issue.
2. Create a branch from the issue.
3. Implement the change on the branch.
4. Commit changes with a message referencing the issue number.
5. Open a Pull Request into `main`.
6. Add `Closes #<issue-number>` to the Pull Request description.
7. Merge the Pull Request after review.

## Commit Messages

Commit messages should clearly describe the implemented change and reference the related issue.

Example:

```text
Add user status field (#5)
