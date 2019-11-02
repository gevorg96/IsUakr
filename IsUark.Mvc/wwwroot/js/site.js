$(document).ready(function(){
    $("select.street").change(function(){
        var selectedStreet = $(this).children("option:selected").val();
        $.ajax({
            type: "GET",
            url: "../api/houses/" + selectedStreet,
            success: function(response) {
                $('#houses').find('option').remove().end();
                for (var i = 0; i <response.length; i++){
                    $('#houses').append('<option value="' + response[i].id +'">'+ response[i].num +'</option>'); 
                }
            },  
            error: function(thrownError) {
                
                console.log(thrownError);
            }
        });
    });
});