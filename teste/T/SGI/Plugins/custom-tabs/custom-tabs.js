function CustomTab() {
    //<construtor>
    confgurarEventos();
    //<eventos>
    function confgurarEventos() {
        $(document).on('click', '.tab-link', clickLinkTab);
    }
    //click links
    function clickLinkTab() { 
        $('.tab-item').find('.show').removeClass('show');
        $(this).addClass('show');
        $('.tab-custom-content').find('div').removeClass('show');
        $('.tab-custom-content ' + $(this).attr('href')).addClass('show');
    }
}
new CustomTab();//instancia a classe