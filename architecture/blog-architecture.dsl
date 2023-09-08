workspace "Blog" "A diagram showing the architecture of the Blog Application" {

    model {
        user = person "User" "Any person who is using Internet. Public in general, a Writer, an Editor."

        group "Blog App" {
		
			softwareSystem = softwareSystem "Blog System" {
				webapp = container "Web App UI" "Provides to the users the content requested via their web browser " "ASP NET Core, Razor, Javascript" {
					user -> this "Visits blog.com, and view, create, edit posts, make and view comments, submit posts, approve posts, reject posts"
				}
				blogapi = container "Blog API" "Provides blog functionalities via JSON/HTTP API" "net 7" {
				
					securityFilter = component "Security Layer" "Filters security access and roles" "net 7" {
						webapp -> this "Makes API calls to" "JSON/HTTPS"
					}
				
					userController = component "User Controller" "Allows users to register" "net 7" {
						securityFilter -> this "Uses"
					}
					userService = component "User Service" "Maintain business rules for actions triggered through the User Controller" "net 7"
					userRepository = component "User Repository" "Interact with the database for reading and writing data related to users" "net 7"
					blogController = component "Blog Controller" "Allows users to view, create, edit posts, make and view comments, submit posts, approve posts, reject posts" "net 7"  {
						securityFilter -> this "Uses"
					}
					blogService = component "Blog Service" "Maintain business rules for actions triggered through the Blog Controller" "net 7"
					blogRepository = component "Blog Repository" "Interact with the database for reading and writing data related to posts and comments" "net 7"
									
					userController -> userService "Uses"
					userService -> userRepository "Uses"
					
					blogController -> blogService "Uses"
					blogService -> blogRepository "Uses"
					
										
				}
				database = container "Database" "Store user data, blog content and comment content" "InMemory" {
					blogRepository -> this "Reads from and writes to" "SQL/TCP"
					userRepository -> this "Reads from and writes to" "SQL/TCP"
				}
			}
        }       
    }

    views {
        systemContext softwareSystem {
            include *
            autolayout lr
        }

        container softwareSystem {
            include *
            autolayout lr
        }
		
        component blogapi "Components" {
            include *
            autoLayout lr
            description "The component diagram for the Blog API."
        }

		theme default
    }
}