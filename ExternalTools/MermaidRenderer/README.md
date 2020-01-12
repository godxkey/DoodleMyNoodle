# Mermaid CLI

## Why do I have to install it myself ?
Github doesn't allow bigger files than 100MB. This is an issue since the mermaid CLI uses a chromium package (contains large binary files) to generate its images.

## How to install
Step 1: Install Node.js from [here](https://nodejs.org/en/)). This should include an installation of NPM
Step 2: Open console like Powershell
Step 3: Make sure the context is in this directory [PROJECT]\ExternalTools\MermaidRenderer directory (the installation needs to be under that folder)
Step 4: Adjust the execution policy if needed (with Set-ExecutionPolicy -ExecutionPolicy Unrestricted -Scope Process)
Step 5: Execute the following lines:
```
npm install mermaid.cli
./node_modules/.bin/mmdc -h
```