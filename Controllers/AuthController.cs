
using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Services;
using RaceDay.Mvc.ViewModels;
using System.Text.Json;

namespace RaceDay.Mvc.Controllers
{
    public class AuthController : Controller
    {
        private readonly RaceDayApiClient _api;

        public AuthController(RaceDayApiClient api)
        {
            _api = api;
        }

        // GET: Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVm vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var request = new
            {
                FirstName = vm.FirstName,
                LastName = vm.LastName,
                Email = vm.Email,
                Password = vm.Password,
                Role = vm.Role
            };

            try
            {
                using var response = await _api.SendAsync(
                    HttpMethod.Post,
                    "api/auth/register",
                    request);

                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError("",
                        await response.Content.ReadAsStringAsync());

                    return View(vm);
                }

                TempData["Success"] =
                    "Registration successful. Please log in.";

                return RedirectToAction(nameof(Login));
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError("",
                    "Could not connect to the RaceDay API.");

                return View(vm);
            }
        }

        // GET: Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVm vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            try
            {
                using var response = await _api.SendAsync(
                    HttpMethod.Post,
                    "api/auth/login",
                    new
                    {
                        Email = vm.Email,
                        Password = vm.Password
                    });

                if (!response.IsSuccessStatusCode)
                {
                    ModelState.AddModelError("",
                        "Incorrect email or password.");

                    return View(vm);
                }

                // Preserve the API session cookie.
                string? cookie =
                    RaceDayApiClient.ExtractSessionCookie(response);

                if (!string.IsNullOrWhiteSpace(cookie))
                {
                    HttpContext.Session.SetString(
                        "ApiSessionCookie", cookie);
                }

                string json =
                    await response.Content.ReadAsStringAsync();

                using JsonDocument document =
                    JsonDocument.Parse(json);

                JsonElement data = document.RootElement;

                // Support responses wrapped in user or data.
                if (TryGetProperty(data, "user", out var user))
                    data = user;
                else if (TryGetProperty(data, "data", out var result))
                    data = result;

                int? userId = ReadInt(data, "UserId", "Id");

                string? role = ReadString(data, "Role");

                string? displayName = ReadString(
                    data,
                    "FirstName",
                    "Username",
                    "Name",
                    "Email");

                if (!userId.HasValue ||
                    string.IsNullOrWhiteSpace(role))
                {
                    ModelState.AddModelError("",
                        "The API login response is missing the user ID or role.");

                    return View(vm);
                }

                HttpContext.Session.SetInt32("UserId", userId.Value);
                HttpContext.Session.SetString("Role", role);
                HttpContext.Session.SetString(
                    "DisplayName", displayName ?? vm.Email);

                if (role.Equals(
                    "Organiser",
                    StringComparison.OrdinalIgnoreCase))
                {
                    return RedirectToAction("Index", "Organiser");
                }

                if (role.Equals(
                    "Participant",
                    StringComparison.OrdinalIgnoreCase))
                {
                    return RedirectToAction("Index", "Participant");
                }

                HttpContext.Session.Clear();

                ModelState.AddModelError("",
                    "Your account has an unrecognised role.");

                return View(vm);
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError("",
                    "Could not connect to the RaceDay API.");

                return View(vm);
            }
            catch (JsonException)
            {
                ModelState.AddModelError("",
                    "The API returned an invalid login response.");

                return View(vm);
            }
        }

        // Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            try
            {
                using var response = await _api.SendAsync(
                    HttpMethod.Post,
                    "api/auth/logout");
            }
            catch (HttpRequestException)
            {
                // Still clear the local session if the API is unavailable.
            }

            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Home");
        }

        private static bool TryGetProperty(
            JsonElement element,
            string name,
            out JsonElement value)
        {
            value = default;

            if (element.ValueKind != JsonValueKind.Object)
                return false;

            foreach (var property in element.EnumerateObject())
            {
                if (property.Name.Equals(
                    name, StringComparison.OrdinalIgnoreCase))
                {
                    value = property.Value;
                    return true;
                }
            }

            return false;
        }

        private static int? ReadInt(
            JsonElement element, params string[] names)
        {
            foreach (string name in names)
            {
                if (TryGetProperty(element, name, out var value))
                {
                    if (value.ValueKind == JsonValueKind.Number &&
                        value.TryGetInt32(out int number))
                        return number;

                    if (int.TryParse(value.ToString(), out number))
                        return number;
                }
            }

            return null;
        }

        private static string? ReadString(
            JsonElement element, params string[] names)
        {
            foreach (string name in names)
            {
                if (TryGetProperty(element, name, out var value))
                    return value.ToString();
            }

            return null;
        }
    }
}
