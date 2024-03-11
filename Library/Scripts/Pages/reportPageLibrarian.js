var reportPageLibrarian = (function () {
    function init($container) {
      
        function loadBookInformation(startDateEntity, endDateEntity, selectedCountOfItemsEntity) {
            $.ajax({
                type: 'POST',
                url: '/Librarian/LoadBookInformation',
                data: {
                    startDate: startDateEntity,
                    endDate: endDateEntity,
                    selectedCountOfItems: selectedCountOfItemsEntity
                },
                success: function (response) {
                    if (response.status) {
                        var books = response.data;

                        var fragment = document.createDocumentFragment();
                        if (books && books.length > 0) {
                            books.forEach(function (book) {

                                var link = document.createElement('a');
                                link.href = '/Home/BookPage?bookId=' + book.id;

                                var imageContainer = document.createElement('div');
                                imageContainer.className = 'd-flex justify-content-center align-items-center mb-2 book-container';
                                imageContainer.style.height = '30rem';
                                imageContainer.style.width = '30rem';
                                imageContainer.style.border = '1px solid #ccc';

                                var image = document.createElement('img');
                                image.src = 'https://res.cloudinary.com/dzaicqbce/image/upload/v1695818842/image-for-book-' + book.id + '.png';
                                image.style.maxWidth = '100%';
                                image.style.maxHeight = '100%';

                                var nameDiv = document.createElement('div');
                                nameDiv.className = 'h3 text-center py-3 text-dark';
                                nameDiv.textContent = book.name;

                                imageContainer.appendChild(image);
                                link.appendChild(imageContainer);
                                link.appendChild(nameDiv);

                                fragment.appendChild(link);
                            });
                        } else {
                            var message = document.createElement('h2');
                            message.textContent = 'Няма записи в базата между тези дати!';
                            message.style.textAlign = 'center';
                            message.style.color = 'red';
                            fragment.appendChild(message);
                        }
                        var container = $('#bookContainer');
                        container.empty();
                        container.append(fragment);
                    } else {
                        console.error(response.Message);
                    }
                },
                error: function (xhr, status, error) {
                    console.error(error);
                }
            });
        }

        $(document).on('change', '#bookSelect', function () {
            if ($(this).val() === 'Personalized') {
                $('#book-personalized-row').show();
            } else {
                $('#book-personalized-row').hide();
                var dateEntity = $(this).val();
                var startDateEntity = '';
                var endDateEntity = '';

                var currentDate = new Date();

                switch (dateEntity) {
                    case 'Today':
                        startDateEntity = getFormattedDateTime(getDateHoursAgo(currentDate, 24));
                        endDateEntity = getFormattedDateTime(currentDate);
                        break;
                    case 'Yesterday':
                        startDateEntity = getFormattedDateTime(getDateHoursAgo(currentDate, 48));
                        endDateEntity = getFormattedDateTime(getDateHoursAgo(currentDate, 24));
                        break;
                    case 'Week':
                        startDateEntity = getFormattedDateTime(getFirstDayOfWeek(currentDate));
                        endDateEntity = getFormattedDateTime(currentDate);
                        break;
                    case 'Month':
                        startDateEntity = getFormattedDateTime(getFirstDayOfMonth(currentDate));
                        endDateEntity = getFormattedDateTime(currentDate);
                        break;
                    case 'Year':
                        startDateEntity = getFormattedDateTime(getFirstDayOfYear(currentDate));
                        endDateEntity = getFormattedDateTime(currentDate);
                        break;
                    default:
                        startDateEntity = getFormattedDateTime(getDateHoursAgo(currentDate, 24));
                        endDateEntity = getFormattedDateTime(currentDate);
                        break;
                }

                var selectedCountOfItemsEntity = $('#amountOfBookItems').val();

                loadBookInformation(startDateEntity, endDateEntity, selectedCountOfItemsEntity);
            }
        });

        $('#personalized-book-time-btn').click(function () {
            var startDateEntity = $('#from-date-book').val();
            var endDateEntity = $('#to-date-book').val();
            var selectedCountOfItemsEntity = $('#amountOfBookItems').val();

            loadBookInformation(startDateEntity, endDateEntity, selectedCountOfItemsEntity);
        });

        $(document).on('change', '#amountOfBookItems', function () {
            var dateEntity = $('#bookSelect').val();
            var startDateEntity = '';
            var endDateEntity = '';

            var currentDate = new Date();

            switch (dateEntity) {
                case 'Today':
                    startDateEntity = getFormattedDateTime(getDateHoursAgo(currentDate, 24));
                    endDateEntity = getFormattedDateTime(currentDate);
                    break;
                case 'Yesterday':
                    startDateEntity = getFormattedDateTime(getDateHoursAgo(currentDate, 48));
                    endDateEntity = getFormattedDateTime(getDateHoursAgo(currentDate, 24));
                    break;
                case 'Week':
                    startDateEntity = getFormattedDateTime(getFirstDayOfWeek(currentDate));
                    endDateEntity = getFormattedDateTime(currentDate);
                    break;
                case 'Month':
                    startDateEntity = getFormattedDateTime(getFirstDayOfMonth(currentDate));
                    endDateEntity = getFormattedDateTime(currentDate);
                    break;
                case 'Year':
                    startDateEntity = getFormattedDateTime(getFirstDayOfYear(currentDate));
                    endDateEntity = getFormattedDateTime(currentDate);
                    break;
                default:
                    startDateEntity = getFormattedDateTime(getDateHoursAgo(currentDate, 24));
                    endDateEntity = getFormattedDateTime(currentDate);
                    break;
            }

            var selectedCountOfItemsEntity = $(this).val();

            loadBookInformation(startDateEntity, endDateEntity, selectedCountOfItemsEntity);
        });

        function getDateHoursAgo(date, hours) {
            return new Date(date.getTime() - (hours * 60 * 60 * 1000));
        }

        function getFirstDayOfWeek(date) {
            var dayOfWeek = date.getDay();
            var diff = date.getDate() - dayOfWeek + (dayOfWeek === 0 ? -6 : 1);
            return new Date(date.setDate(diff));
        }

        function getFirstDayOfMonth(date) {
            return new Date(date.getFullYear(), date.getMonth(), 1);
        }

        function getFirstDayOfYear(date) {
            return new Date(date.getFullYear(), 0, 1);
        }

        function getFormattedDateTime(date) {
            var year = date.getFullYear();
            var month = (date.getMonth() + 1).toString().padStart(2, '0');
            var day = date.getDate().toString().padStart(2, '0');
            var hours = date.getHours().toString().padStart(2, '0');
            var minutes = date.getMinutes().toString().padStart(2, '0');
            return year + '-' + month + '-' + day + 'T' + hours + ':' + minutes;
        }

        var isChartLoading = false; 

        $(document).on('change', '#genreSelect', function () {
            if ($(this).val() === 'Personalized') {
                $('#genre-personalized-row').show();
                $('#personalized-genre-time-btn').trigger('click');
            } else {
                $('#genre-personalized-row').hide();
                var dateEntity = $(this).val();
                var startDateEntity = '';
                var endDateEntity = '';

                var currentDate = new Date();

                switch (dateEntity) {
                    case 'Today':
                        startDateEntity = getFormattedDateTime(getDateHoursAgo(currentDate, 24));
                        endDateEntity = getFormattedDateTime(currentDate);
                        break;
                    case 'Yesterday':
                        startDateEntity = getFormattedDateTime(getDateHoursAgo(currentDate, 48));
                        endDateEntity = getFormattedDateTime(getDateHoursAgo(currentDate, 24));
                        break;
                    case 'Week':
                        startDateEntity = getFormattedDateTime(getFirstDayOfWeek(currentDate));
                        endDateEntity = getFormattedDateTime(currentDate);
                        break;
                    case 'Month':
                        startDateEntity = getFormattedDateTime(getFirstDayOfMonth(currentDate));
                        endDateEntity = getFormattedDateTime(currentDate);
                        break;
                    case 'Year':
                        startDateEntity = getFormattedDateTime(getFirstDayOfYear(currentDate));
                        endDateEntity = getFormattedDateTime(currentDate);
                        break;
                    default:
                        startDateEntity = getFormattedDateTime(getDateHoursAgo(currentDate, 24));
                        endDateEntity = getFormattedDateTime(currentDate);
                        break;
                }

                // Check if a chart is currently being loaded
                if (!isChartLoading) {
                    isChartLoading = true; // Set the flag to true to indicate that a chart is being loaded

                    $.ajax({
                        type: 'POST',
                        url: '/Librarian/LoadGenreInformation',
                        data: {
                            startDate: startDateEntity,
                            endDate: endDateEntity,
                        },
                        success: function (response) {
                            if (response.status) {
                                var genresData = response.data;
                                var canvas = document.getElementById('genreChart');
                                var ctx = canvas.getContext('2d');
                                ctx.clearRect(0, 0, canvas.width, canvas.height);
                                if (genresData && genresData.length > 0) {
                                    $('#no-chart-items-in-db').hide();
                                    $('#genreChart').show();
                                    loadGenreChart(genresData);
                                }
                                else {
                                    $('#no-chart-items-in-db').show();
                                    $('#genreChart').hide();
                                }
                            } else {
                                console.error(response.Message);
                            }

                            isChartLoading = false; // Reset the flag after chart loading is complete
                        },
                        error: function (xhr, status, error) {
                            console.error(error);
                            isChartLoading = false; // Reset the flag in case of an error
                        }
                    });
                }
            }
        });

        $('#personalized-genre-time-btn').click(function () {
            var startDateEntity = $('#from-date-genre').val();
            var endDateEntity = $('#to-date-genre').val();

            if (!isChartLoading) {
                isChartLoading = true; 

                $.ajax({
                    type: 'POST',
                    url: '/Librarian/LoadGenreInformation',
                    data: {
                        startDate: startDateEntity,
                        endDate: endDateEntity,
                    },
                    success: function (response) {
                        if (response.status) {
                            var genresData = response.data;

                            var canvas = document.getElementById('genreChart');
                            var ctx = canvas.getContext('2d');
                            ctx.clearRect(0, 0, canvas.width, canvas.height);
                            if (genresData && genresData.length > 0) {
                                $('#no-chart-items-in-db').hide();
                                $('#genreChart').show();
                                loadGenreChart(genresData);
                            }
                            else {
                                $('#no-chart-items-in-db').show();
                                $('#genreChart').hide();
                            }
                        } else {
                            console.error(response.Message);
                        }

                        isChartLoading = false; 
                    },
                    error: function (xhr, status, error) {
                        console.error(error);
                        isChartLoading = false; 
                    }
                });
            }
        });

        function loadGenreChart(genresData) {
            var canvas = document.getElementById('genreChart');
            var ctx = canvas.getContext('2d');

            // Clear the canvas
            ctx.clearRect(0, 0, canvas.width, canvas.height);

            // Destroy previous chart instance if it exists
            if (window.myChart) {
                window.myChart.destroy();
            }

            var genres = [];
            var percentages = [];
            var colors = [];

            genresData.forEach(function (item) {
                var parts = item.split('-');
                var genreName = parts[0].trim();
                var percentage = parseFloat(parts[1].trim());

                genres.push(genreName);
                percentages.push(percentage);

                var randomColor = '#' + Math.floor(Math.random() * 16777215).toString(16);
                colors.push(randomColor);
            });

            window.myChart = new Chart(ctx, {
                type: 'pie',
                data: {
                    labels: genres,
                    datasets: [{
                        label: 'Percentage',
                        data: percentages,
                        backgroundColor: colors,
                        borderWidth: 1
                    }]
                },
                options: {
                    plugins: {
                        datalabels: {
                            formatter: (value, ctx) => {
                                let sum = 0;
                                let dataArr = ctx.chart.data.datasets[0].data;
                                dataArr.map(data => {
                                    sum += data;
                                });
                                let percentage = (value * 100 / sum).toFixed(2) + "%";
                                return percentage;
                            },
                            color: '#fff',
                        }
                    },
                    legend: {
                        display: true,
                        position: 'left',
                        align: 'start',
                        labels: {
                            boxWidth: 10
                        },
                        title: {
                            display: true,
                            text: 'Legend'
                        }
                    }
                }
            });
        }

    }
    return {
        init
    };
})();