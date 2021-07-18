using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TweetbookApi.Contracts.V1.Requests;
using TweetbookApi.Controllers.V1.Dtos;
using TweetbookApi.Models;
using TweetbookApi.Services;

namespace TweetbookApi.Controllers.V1
{
    public class PostController : ControllerBaseV1
    {

        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var posts = await _postService.GetAll();
            var response = new List<PostResponse>();

            foreach (var post in posts)
            {
                response.Add(new PostResponse { Id = post.Id, Name = post.Name });
            }

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Detail(string id)
        {
            var post = await _postService.GetById(id);

            if (post == null)
                return NotFound();

            var response = new PostResponse { Id = post.Id, Name = post.Name };

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePostRequest request)
        {
            if (string.IsNullOrEmpty(request.Id))
                return BadRequest();

            var newPost = new Post { Id = request.Id, Name = request.Name };

            var createdPost = await _postService.Add(newPost);

            var response = new PostResponse { Id = createdPost.Id, Name = createdPost.Name };

            return CreatedAtAction(nameof(Detail), new { Id = newPost.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, UpdatePostRequest request)
        {
            if (request.Id != id)
                return BadRequest();

            var exits = await _postService.GetById(id) != null;

            if (!exits)
                return NotFound();

            var post = new Post { Id = request.Id, Name = request.Name };

            var updated = await _postService.Update(post);

            if (!updated)
                return StatusCode(((int)HttpStatusCode.InternalServerError));

            return NoContent();
        }
    }
}