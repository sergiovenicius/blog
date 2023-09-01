# blog api

# contact
sergiovenicius.chapeco@gmail.com

# requirements
- dot net 7
- Visual Studio (Community) 2022
- dotnet stryker 3.10 (stryker mutator) -> https://stryker-mutator.io/docs/stryker-net/getting-started/
- k6.io (load tests) -> https://k6.io/docs/get-started/installation/

# steps to build
- clone the repository to your machine
- open the solution "Blog.sln" in your Visual Studio
- click to Build and Run

# details about blog.api
- the project "blog.api" was built using net 7;
- it was configured to use inMemory database;
- the code-coverage of the project is 100%. 
- mutation tests analysis covered 90.10%
- it is configured to run always at: http://localhost:5000
- the swagger is fully documented, you can access it while in Debug at http://localhost:5000/swagger
- the user apis doesn't require authentication
- the post apis requires a Basic Authorization - you must send the header as: Authorization: Basic base64(username:pwd)
- the file "summary.html" shows the reports related to the k6 script (in the repository) - load_test_api.js
- you can re-run k6 script using the command line (at the root folder of the repo): k6 run load_test_api.js
- the file "mutation-report.html" under the folder "reports" has the results of the mutation tests possibilities generated by dotnet-stryker
- you can re-run dotnet-stryker for mutation tests possibilities using the command line (at the root folder of the repo): dotnet-stryker -s Blog.sln -O .

# total hours worked
16h
