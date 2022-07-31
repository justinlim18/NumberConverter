using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NumberConverter.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace NumberConverter.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        private string[] teens = { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", 
                                   "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
        private string[] twos = { "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };
        private string[] tens = { "thousand", "million" };
        // thousands, millions

        public IActionResult Index()
        {
            string html = "<form method='post' action='/home/numberconvert/'>" +
                "<h1>Enter a number (in digits) to convert</h1>" +
                "<input type='text' name='number' />" +
                "<input type='submit' value='Convert'/>" +
                "</form>";
            //return View();
            return Content(html, "text/html");
        }

        public IActionResult numberconvert(string number)
        {
            string conv_numbers = "{0}";
            string conv_numbers_hold = "";
            string[] numbers = number.Split('.');

            int count = 0;
            int count_copy = 0;
            for (int doll_cents = 0; doll_cents < numbers.Length; doll_cents++)
            {
                // split into three digits (every hundred)
                if (numbers[doll_cents].Length % 3 == 0)
                {
                    count = numbers[doll_cents].Length / 3;
                }
                else
                {
                    count = numbers[doll_cents].Length / 3 + 1;
                }
                count_copy = count;

                for (int i = numbers[doll_cents].Length - 1; i >= 0; i -= 3)
                {
                    // hundred, ten, one digits
                    int[] hold = { 0, 0, 0 };
                    int k = 2;

                    for (int j = i; j >= i - 2; j--)
                    {
                        if (j < 0)
                        {
                            break;
                        }
                        hold[k] = numbers[doll_cents][j] - '0';
                        k--;
                    }
                    // thousands, millions, billions
                    if (i - 2 > 0)
                    {
                        conv_numbers_hold += "{0} ";
                        conv_numbers_hold += tens[count_copy - count] + ' ';
                        count--;
                    }
                    // Hundred
                    if (hold[0] != 0)
                    {
                        conv_numbers_hold += teens[hold[0] - 1] + ' ';
                        conv_numbers_hold += "hundred ";
                        if (hold[1] != 0 && i == numbers[doll_cents].Length - 1)
                        {
                            conv_numbers_hold += "and ";
                        }
                    }
                    // Less than 20
                    if (hold[1] < 2)
                    {
                        int ind = 0;
                        if (hold[1] != 0)
                        {
                            ind = hold[1] + 9;
                        }
                        ind += hold[2];
                        if (ind != 0)
                        {
                            conv_numbers_hold += teens[ind - 1] + ' ';
                        }
                    }
                    // Greater than 20
                    else
                    {
                        conv_numbers_hold += twos[hold[1] - 2];
                        if (hold[2] != 0)
                        {
                            conv_numbers_hold += '-' + teens[hold[2] - 1] + ' ';
                        }
                    }

                    conv_numbers = String.Format(conv_numbers, conv_numbers_hold);
                    conv_numbers_hold = "";
                }

                if (doll_cents == 0)
                {
                    conv_numbers += "dollars";
                    if (numbers.Length == 2)
                    {
                        conv_numbers += ", {0}";
                    }
                } 
                else
                {
                    conv_numbers += "cents";
                }
            }
            
            string html = "<h1>Number is: $" + number + "</h1>";
            html += "<h1>Convertered number is: " + conv_numbers + "</h1>";
            return Content(html, "text/html");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
