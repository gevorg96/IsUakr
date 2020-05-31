$(document).ready(function(){
    $("select.street").change(function(){
        var selectedStreet = $(this).children("option:selected").val();
        $.ajax({
            type: "GET",
            url: "../api/houses/" + selectedStreet,
            success: function(response) {
                $('#houses').find('option').remove().end();
                for (var i = 0; i <response.length; i++){
                    $('#houses').append('<option value="' + response[i].id +'">'+ response[i].number +'</option>'); 
                }
                refreshMeters();
            },  
            error: function(thrownError) {
                
                console.log(thrownError);
            }
        });
    });
});


$(document).ready(function(){
    $("select.house").change(function(){
        refreshMeters();
    });
});

function refreshMeters() {
    var selectedHouse = $("select.house").children("option:selected").val();
    $.ajax({
        type: "GET",
        url: "../api/meters/" + selectedHouse,
        success: function(response) {
            $('#meterHub').find('th').remove().end();
            $('#meterHub').find('td').remove().end();
            $('#meterHub').append('<th scope="row">' + response.id +'</th>');
            $('#meterHub').append('<td>' + response.code +'</td>');

            $('#meterTable').find('th').remove().end();
            $('#meterTable').find('td').remove().end();

            var flats = response.flats;
            for (var i = 0; i < flats.length; i++){
                var meters = flats[i].meters;
                for (var j = 0; j < meters.length; j++) 
                {
                    var type = meters[j].type == "heat_water" ? "ГВС" : meters[j].type == "cold_water" ? "ХВС" : "Электр.";
                    $('#meterTable').append('<tr><th scope="row">' + meters[j].id + '</th>' + (j == 0 ? '<td class="align-middle" rowspan="' + meters.length+ '">кв. '+ flats[i].num +'</td>': "" )+'<td>' + meters[j].code + '</td><td>'+type+'</td></tr>');
                }
                
            }
        },
        error: function(thrownError) {

            console.log(thrownError);
        }
    });
}