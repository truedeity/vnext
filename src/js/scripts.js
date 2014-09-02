(function ($, window, document, undefined) {

  'use strict';

  $(function () {
      $(".menu-link").click(function () {
          $("#menu").toggleClass("active");
          $(".container").toggleClass("active");
      });
  });

})(jQuery, window, document);
