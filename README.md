# blog project (api, ui and api.test)

# contact
sergiovenicius.chapeco@gmail.com

# business rules
- the business rules were sent as .net technical test. It is basically: "Build a minimal Blog Engine / CMS backend API, that allows to create, edit and publish textbased posts, with an approval flow where two different user types may interact."
- the business requirements are described here: https://github.com/sergiovenicius/blog/blob/main/NET_Test.pdf

# requirements
- dot net 7
- ef core 7
- Visual Studio (Community) 2022
- dotnet stryker 3.10 (stryker mutator) -> https://stryker-mutator.io/docs/stryker-net/getting-started/
- k6.io (load tests) -> https://k6.io/docs/get-started/installation/

# architecture
- you can load the architecture/blog-architecture.dsl file into https://structurizr.com/dsl to see context, container and component diagrams

# steps to build
- clone the repository to your machine
- open the solution "Blog.sln" in your Visual Studio
- click to Build and Run

# details about blog.api
- the project "blog.api" was built using net 7;
- it was configured to use inMemory database;
- the code-coverage of the project is 100%. 
- mutation tests analysis covered 87.80%
- it is configured to run always at: http://localhost:5000
- the swagger is fully documented, you can access it while in Debug at http://localhost:5000/swagger
- the user apis doesn't require authentication
- the post apis requires a Basic Authorization - you must send the header as: Authorization: Basic base64(username:pwd)
- the file "summary.html" shows the reports related to the k6 script (in the repository) - load_test_api.js
- you can re-run k6 script using the command line (at the root folder of the repo): k6 run load_test_api.js
- the file "mutation-report.html" under the folder "reports" has the results of the mutation tests possibilities generated by dotnet-stryker
- you can re-run dotnet-stryker for mutation tests possibilities using the command line (at the root folder of the repo): dotnet-stryker -s Blog.sln -O .

# details about blog.ui
- blog.ui will start along with blog.api when you click to Build and Run. It is an user interface to interact a bit with the api.
- when the blog.ui starts, it registers 3 users, each one with a different permission.
- in the first screen you will see a table with the usernames and passwords and roles. You can login with any of them.
- after login, you will see a screen that shows all published posts. There is an option in each line to see details, which shows all the post details, including comments.
- the other interactions must be done using swagger, where it's a bit described below.

# guide
- to interact with the api, you can use swagger. Here are some examples:
  - post a new user. Roles are: 0 (public), 1 (Writer), 2 (Editor)
  - encode as base64 the username:password of the user you want to do the current request
  - at the top-right there is a button credentials, you must fill with Basic base64 from previous step
  - if you are a Writer, you can add new posts, list your own posts, and also submit them, for example;
  - if you are an Editor, you can list pending posts, approve, reject other posts, for example;
  - if you are public, you can list all published posts and get a post by its ID, for example;

# improvements
- search and pagination
- load tests
- when rejecting a post, it's being required to post a comment along with
- other features in the UI to interact with the api
- exception specific classes

# total hours worked
20h

#
