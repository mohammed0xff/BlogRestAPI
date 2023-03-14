# BlogRestAPI
![MicrosoftSQLServer](https://img.shields.io/badge/Microsoft%20SQL%20Sever-CC2927?style=for-the-badge&logo=microsoft%20sql%20server&logoColor=white)
![Swagger](https://img.shields.io/badge/-Swagger-%23Clojure?style=for-the-badge&logo=swagger&logoColor=white)
![JWT](https://img.shields.io/badge/JWT-black?style=for-the-badge&logo=JSON%20web%20tokens)

Restful Blog API with jwt authentication.

## Features
#### Users can :
* Create and Follow blogs.
* Add posts to their blogs.
* Like and Comment to posts.
* Add tags to their posts.
* Change profile picture
* Change username

#### Admins can : 
* Suspend/Unsuspend users.

## Controllers 

### Auth

| Method | Route | 
| ------ | ------------------ |
| POST   | /api/auth/signup |
| POST   | /api/auth/signin |
| POST   | /api/auth/signout |
| POST   | /api/auth/refreshToken |

### Users
| Method | Route | 
| ------ | ------------------ |
| GET | ​/api​/user |
| POST | /api​/user​/change-profile-photo|
| POST | ​/api​/user​/change-username|
| DELETE | ​/api​/user​/remove-profile-photo|
| GET | ​/api​/user​/users-list|

### Admins
| Method | Route | 
| ------ | ------------------ |
| GET | ​/api​/adminontroller​/suspend-user​/{username} |
| GET | ​/api​/adminontroller​/unsuspend-user​/{username}|

### Blogs

| Method | Route | Description | 
| ------ | --- | ----------- |
| GET    | /api/blogs/ | Get all blogs | 
| GET    | /api/blogs/{id} | Get blog by id | 
| POST   | /api/blogs | Create new blog |
| PUT    | /api/blogs/{id} | Update blog  | 
| DELETE | /api/blogs/{id} | Delete blog |
| GET    | /api/blogs/{id}/followers | Get blog by id | 
| POST | /api/blogs/{id}/follow | Follow blog |
| POST | /api/blogs/{id}/unfollow | Unfollow blog |
| GET | /api/blogs/followed-blogs | Get followed blogs |

### Posts

| Method | Route | Description | 
| ------ | --- | ----------- |
| GET | /api/posts | Get all posts certain Blog|
| GET | /api/posts/{postId} | Get post by id |
| POST | /api/posts | Add new post |
| PUT | /api/posts/{postId} | Update post |
| DELETE | /api/posts/{postId} | Delete post |
| POST | /api/posts/{postId}/like | Like a post |
| POST | /api/posts/{postId}/unlike | Remove like of a post |
| GET | /api/posts/{postId}/likes | Get all likes by `postId`|


### Comments

| Method | Route | Description |
| ------ | --- | ----------- |
| GET | /api/comments | Get all comments of a certain Post | 
| POST | /api/comments | Add new comment |
| PUT | /api/comments/{commentId} | Update a commment |
| DELETE | /api/comments/{commentId} | Delete a comment |
| POST | /api/comments/{commentId}/like | Like a comment |
| POST | /api/comments/{commentId}/unlike | Remove like of a commment |

### Tags
| Method | Route | Description |
| ------ | --- | ----------- |
| GET | /api/tags | Get all tags| 
| POST | /api/tags | Add new tag |

<br/>

## Testing 
You can either register and login 
or use any of the two seeded users :
* Email : `johndoe@gmail.com` password : `Passwd@123`
* Email : `janedoe@gmail.com` password : `Passwd@123` 
* Email : `admin@gmail.com` password : `Passwd@123` 

to gain jwt access.

<br />
You can use with `curl` , postman or head to `https://localhost:7086/swagger/index.html`
<br />
Have fun! :smiley:

## License

[![Licence](https://img.shields.io/github/license/Ileriayo/markdown-badges?style=for-the-badge)](./LICENSE)
