# BlogRestAPI
Restful Blog API with jwt authentication.


## Controllers 

### Auth

| Method | Route | 
| ------ | ------------------ |
| POST   | /api/auth/signup |
| POST   | /api/auth/signin |
| POST   | /api/auth/signout |
| POST   | /api/auth/refreshToken |


### Blogs

| Method | Route | Description | 
| ------ | --- | ----------- |
| GET    | /api/blogs/ | Get all blogs | 
| GET    | /api/blogs/{id} | Get blog by id | 
| GET    | /api/blogs/page | Get paged result by requiring <br> `pageNumber?` and `pageSize?` as query params | 
| POST   | /api/blogs | Create new blog |
| PUT    | /api/blogs/{id} | Update blog  | 
| DELETE | /api/blogs/{id} | Delete blog |


### Posts

| Method | Route | Description | 
| ------ | --- | ----------- |
| GET | /api/blogs/{blogId}/posts | Get all posts of a certain blog   |
| GET | /api/blogs/blogId/posts/page | Get paged result by requiring <br> `pageNumber?` and `pageSize?` as query params   |
| GET | /api/blogs/{blogId}/posts/{postId} | Get post by id |
| POST | /api/blogs/{blogId}/posts | Add new post |
| PUT | /api/blogs/{blogId}/posts/{postId} | Update post |
| DELETE | /api/blogs/{blogId}/posts/{postId} | Delete post |
| POST | /api/posts/{postId}/like | Like a post |
| POST | /api/posts/{postId}/unlike | Remove like of a post |
| GET | /api/posts/{postId}/likes | Get all likes by `postId`|


### Comments

| Method | Route | Description |
| ------ | --- | ----------- |
| GET | /api/posts/{postId}/comments | Get all comments of a certain Post | 
| POST | /api/posts/{postId}/comments | Add new comment |
| PUT | /api/posts/{postId}/comments/{commentId} | Update a commment |
| DELETE | /api/posts/{postId}/comments/{commentId} | Delete a comment |
| POST | /api/posts/{postId}/comments/{commentId}/like | Like a comment |
| POST | /api/posts/{postId}/comments/{commentId}/unlike | Remove like of a commment |


