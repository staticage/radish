﻿using System.Net;
using FluentAssertions;
using NUnit.Framework;
using Radish.Helpers;

namespace Radish.UnitTests
{
    public class HttpServerTests
    {
        const int port = 9000;

        [Test]
        public void when_not_matched_should_return_not_found()
        {
            // Arrange
            var server = new HttpServer();
            var engine = HttpServerEngine.StartNew(server, port);

            // Act
            var exception = Assert.Throws<WebException>(() => Http.Get("http://localhost:9000"));
            var response = exception.Response as HttpWebResponse;
            engine.Stop();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Test]
        public void should_return_expected_response_text()
        {
            // Arrange
            var server = new HttpServer();
            server.Response(response => response.Text("test"));
            var engine = HttpServerEngine.StartNew(server, port);

            // Act
            var result = Http.Get("http://localhost:9000");
            engine.Stop();

            // Assert
            result.Should().Be("test");
        }

        [Test]
        public void should_return_expected_response_from_file()
        {
            // Arrange
            var server = new HttpServer();
            server.Response(response => response.File("test.txt"));
            var engine = HttpServerEngine.StartNew(server, port);

            // Act
            var result = Http.Get("http://localhost:9000");
            engine.Stop();

            // Assert
            result.Should().Be("hello");
        }

        [Test]
        public void should_match_request_based_on_specified_uri()
        {
            // Arrange
            var server = new HttpServer();
            server.Request(request => request.Uri.Is("/home"))
                  .Response(response => response.Text("foo"));
            var engine = HttpServerEngine.StartNew(server, port);

            // Act
            var result = Http.Get("http://localhost:9000/home");
            engine.Stop();

            // Assert
            result.Should().Be("foo");
        }

        [Test]
        public void should_match_request_based_on_specified_content()
        {
            // Arrange
            var server = new HttpServer();
            server.Request(request => request.Content.Is("content"))
                  .Response(response => response.Text("foo"));
            var engine = HttpServerEngine.StartNew(server, port);

            // Act
            var result = Http.Post("http://localhost:9000", "content");
            engine.Stop();

            // Assert
            result.Should().Be("foo");
        }

        [Test]
        public void should_match_request_based_on_specified_regex()
        {
            // Arrange
            var server = new HttpServer();
            server.Request(request => request.Uri.Match("^/home"))
                  .Response(response => response.Text("foo"));
            var engine = HttpServerEngine.StartNew(server, port);

            // Act
            var result = Http.Post("http://localhost:9000/home123", "content");
            engine.Stop();

            // Assert
            result.Should().Be("foo");
        }

        [Test]
        public void should_match_request_based_on_either_matcher()
        {
            // Arrange
            var server = new HttpServer();
            server.Request(request => request.Uri.Is("/home") & request.Content.Is("content"))
                  .Response(response => response.Text("foo"));
            var engine = HttpServerEngine.StartNew(server, port);

            // Act
            var result = Http.Post("http://localhost:9000/home", "content");
            engine.Stop();

            // Assert
            result.Should().Be("foo");
        }
    }
}
