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


## Samples and Explorations using `curl`

### ** Auth ** 

#### Request `/api/Auth/login`
```bash
curl -X 'POST' \
  'https://localhost:7086/api/Auth/login' \
  -H 'accept: */*' \
  -H 'Content-Type: application/json' \
  -d '{
  "email": "johndoe@gmail.com",
  "password": "john123"
}
```

#### Response 

```json 
{
  "isAuthenticated": true,
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJqb2huMTIzIiwianRpIjoiNjUzZDAyMjMtZWJlMS00YWM0LTg0MDMtYTViNGJiZmUwNzMzIiwiZW1haWwiOiJqb2huZG9lQGdtYWlsLmNvbSIsInVpZCI6IjZhMDRlNWI2LWI2ZTItNGNjZi1hYTdlLTI0MzA4YzEwZjc2MCIsInJvbGVzIjoiVXNlciIsImV4cCI6MTY2NDU3ODIyOCwiaXNzIjoiU2VjdXJlQXBpIiwiYXVkIjoiU2VjdXJlQXBpVXNlciJ9.g6P4STn00F07AgpUwGx6u66UMoYlkf3cWJ77S4uZbxI",
  "refreshToken": "VNJ5BN6LKO7BK9EMHIZ5NLKPWJ7C776AMM53e28d0ab-2d49-4ff8-9ad3-335685d208f2",
  "expiresOn": "2022-03-30T22:50:28Z",
  "errorMessage": ""
}

```

<br/>

---

### ** Refresh token **

#### Request `/api/Auth/refreshToken`

```bash
curl -X 'POST' \
  'https://localhost:7086/api/Auth/refreshToken' \
  -H 'accept: */*' \
  -H 'Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJqb2huMTIzIiwianRpIjoiNjUzZDAyMjMtZWJlMS00YWM0LTg0MDMtYTViNGJiZmUwNzMzIiwiZW1haWwiOiJqb2huZG9lQGdtYWlsLmNvbSIsInVpZCI6IjZhMDRlNWI2LWI2ZTItNGNjZi1hYTdlLTI0MzA4YzEwZjc2MCIsInJvbGVzIjoiVXNlciIsImV4cCI6MTY2NDU3ODIyOCwiaXNzIjoiU2VjdXJlQXBpIiwiYXVkIjoiU2VjdXJlQXBpVXNlciJ9.g6P4STn00F07AgpUwGx6u66UMoYlkf3cWJ77S4uZbxI' \
  -H 'Content-Type: application/json' \
  -d '{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJqb2huMTIzIiwianRpIjoiNjUzZDAyMjMtZWJlMS00YWM0LTg0MDMtYTViNGJiZmUwNzMzIiwiZW1haWwiOiJqb2huZG9lQGdtYWlsLmNvbSIsInVpZCI6IjZhMDRlNWI2LWI2ZTItNGNjZi1hYTdlLTI0MzA4YzEwZjc2MCIsInJvbGVzIjoiVXNlciIsImV4cCI6MTY2NDU3ODIyOCwiaXNzIjoiU2VjdXJlQXBpIiwiYXVkIjoiU2VjdXJlQXBpVXNlciJ9.g6P4STn00F07AgpUwGx6u66UMoYlkf3cWJ77S4uZbxI",
  "refreshToken": "VNJ5BN6LKO7BK9EMHIZ5NLKPWJ7C776AMM53e28d0ab-2d49-4ff8-9ad3-335685d208f2"
}'
```


#### Response 

```json
{
  "isAuthenticated": true,
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJqb2huMTIzIiwianRpIjoiNmI3NGQyYjEtY2RhMy00N2E3LWJmODgtM2I3ODY3Yjc5MGE1IiwiZW1haWwiOiJqb2huZG9lQGdtYWlsLmNvbSIsInVpZCI6IjZhMDRlNWI2LWI2ZTItNGNjZi1hYTdlLTI0MzA4YzEwZjc2MCIsInJvbGVzIjoiVXNlciIsImV4cCI6MTY2NDU3ODM0MCwiaXNzIjoiU2VjdXJlQXBpIiwiYXVkIjoiU2VjdXJlQXBpVXNlciJ9.W-risEPB4PiT0sZ6addpMt_p2punAIDKXKNB5EGxptY",
  "refreshToken": "PR7N2NGJGSFBZ1KMBBO279CK2WQQJUSOHW4cf477033-6650-4cc2-847f-031f3a7804e3",
  "expiresOn": "2022-03-30T22:52:20Z",
  "errorMessage": ""
}
```

<br/>

---

### ** Blogs **

#### Request `/api/blogs`

```bash 
curl -X 'GET' \
  'https://localhost:7086/api/blogs' \
  -H 'accept: */*'
```
#### Response 

```json 
[
  {
    "id": 1,
    "title": "John's blog",
    "description": "Welcome to my blog",
    "followersCount": 3,
    "user": {
      "fullName": "john",
      "userName": "doe",
      "email": "johndoe@gmail.com"
    }
  },
    {
    "id": 2,
    "title": "Jane's blog",
    "description": "Welcome to my blog",
    "followersCount": 1,
    "user": {
      "fullName": "jane",
      "userName": "doe",
      "email": "janedoe@gmail.com"
    }
  }
]
  
```

<br/>

---

### ** Posts **

#### Request `/api/blogs/2/posts`

```bash 
curl -X 'GET' \
  'https://localhost:7086/api/blogs/2/posts' \
  -H 'accept: */*'
```

#### Response 

```json 
  {
    "id": 3,
    "blogId": 2,
    "headLine": "A new post",
    "content": "Whats up!",
    "commentsAllowed": true,
    "likesCount": 5,
    "datePublished": "2022-03-23T00:32:29.943"
  },
```

<br/>

---

### ** Comments **

#### Request `/api/posts/4/comments`

```bash 
curl -X 'GET' \
  'https://localhost:7086/api/posts/4/comments' \
  -H 'accept: */*'
```

#### Response 

```json 
  {
    "id": 2,
    "content": "Awesome post.",
    "datePublished": "2022-03-22T22:47:15.473",
    "likesCount": 2,
    "isLiked": false,
    "postId": 4,
    "user": {
      "fullName": "John Doe",
      "userName": "John123",
      "email": "johndoe@gmail.com"
    }
  }
```
