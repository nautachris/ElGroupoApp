$(document).ready(function(){
    //$("h1").click(function(){
   //     $(this).css('background-color', '#ff0000');
    //});
    
    //hiddden until we get to the fast food
    var waypoints = $(".js--section-features").waypoint(function(direction) {
        if (direction === 'down'){
            $('nav').addClass('sticky');
        }
        else{
            $('nav').removeClass('sticky');
        }
        
    }, { offset: '60px'});
    
    
    $(".js--scroll-to-plans").on('click', function(){
        $('html, body').animate({scrollTop: $('.js--section-plans').offset().top}, 500);
        
    });
    $(".js--scroll-to-more").on('click', function(){
        $('html, body').animate({scrollTop: $('.js--section-features').offset().top}, 500);

    });
    
    //animate.css github
    
$('a[href*="#"]').not('[href="#"]').not('[href="#0"]').click(function(event) {
    console.log('link click');
    // On-page links
    if (
      location.pathname.replace(/^\//, '') == this.pathname.replace(/^\//, '') 
      && 
      location.hostname == this.hostname
    ) {
      // Figure out element to scroll to
      var target = $(this.hash);
      target = target.length ? target : $('[name=' + this.hash.slice(1) + ']');
      // Does a scroll target exist?
      if (target.length) {
        // Only prevent default if animation is actually gonna happen
        event.preventDefault();
        $('html, body').animate({
          scrollTop: target.offset().top
        }, 1000, function() {
          // Callback after animation
          // Must change focus!
          var $target = $(target);
          $target.focus();
          if ($target.is(":focus")) { // Checking if the target was focused
            return false;
          } else {
            $target.attr('tabindex','-1'); // Adding tabindex for elements not focusable
            $target.focus(); // Set focus again
          };
        });
      }
    }
  });
    
    
    $(".js--wp-1").waypoint(function(direction) {
        $(".js--wp-1").addClass('animated fadeIn');      
    },{
        offset: '50%'
    });
    $(".js--wp-2").waypoint(function(direction) {
        $(".js--wp-2").addClass('animated fadeInUp');
        
    },{
        offset: '50%'
    });
    $(".js--wp-3").waypoint(function(direction) {
        $(".js--wp-3").addClass('animated fadeIn');
        
    },{
        offset: '50%'
    });
    $(".js--wp-4").waypoint(function(direction) {
        $(".js--wp-4").addClass('animated pulse');
        
    },{
        offset: '50%'
    });
    
    $(".js--nav-icon").on('click', function(){
        var nav = $(".js--main-nav");
        var icon = $('i.js--nav-icon');
        console.log('length of icon selector ' + icon.length.toString());
        nav.slideToggle(200);
        if (icon.hasClass('ion-navicon-round')){
            console.log('has ion-navicon-round')
            icon.addClass('ion-close-round');
            icon.removeClass('ion-navicon-round');
        }
        else{
            console.log('has ion-close-round')
            icon.removeClass('ion-close-round');
            icon.addClass('ion-navicon-round');
        }
    });
    
});