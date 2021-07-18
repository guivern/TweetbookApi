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
            var posts = await _postService.GetAllAsync();
            var response = new List<PostResponse>();

            foreach (var post in posts)
            {
                response.Add(new PostResponse { Id = post.Id, Name = post.Name });
            }

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Detail(int id)
        {
            var post = await _postService.GetByIdAsync(id);

            if (post == null)
                return NotFound();

            var response = new PostResponse { Id = post.Id, Name = post.Name };

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePostRequest request)
        {
            var newPost = new Post { Name = request.Name };

            var createdPost = await _postService.CreateAsync(newPost);

            var response = new PostResponse { Id = createdPost.Id, Name = createdPost.Name };

            return CreatedAtAction(nameof(Detail), new { Id = newPost.Id }, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdatePostRequest request)
        {
            if (request.Id != id)
                return BadRequest();

            var exits = await _postService.ExistsAsync(id);

            if (!exits)
                return NotFound();

            var post = new Post { Id = request.Id, Name = request.Name };

            await _postService.UpdateAsync(post);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _postService.GetByIdAsync(id);

            if (post == null)
                return NotFound();

            await _postService.DeleteAsync(post);

            return NoContent();
        }
    }
}