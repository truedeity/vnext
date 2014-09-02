/*!
 * brickpile
 * BrickPile is an open source content management system built on RavenDB 3 and ASP.NET MVC 6
 * https://github.com/brickpile/brickpile
 * @author Marcus Lindblom
 * @version 1.0.0
 * Copyright 2014. MIT licensed.
 */
(function ($, window, document, undefined) {

  'use strict';

  $(function () {
      $(".menu-link").click(function () {
          $("#menu").toggleClass("active");
          $(".container").toggleClass("active");
      });
  });

})(jQuery, window, document);
