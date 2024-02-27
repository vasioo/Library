/*CSS and JS bundling*/

var gulp = require("gulp"),
    less = require("gulp-less"),
    concat = require("gulp-concat"),
    lessToVars = require('gulp-less-vars-to-js'),
    rename = require('gulp-rename'),
    replace = require('gulp-replace');


//common.js
gulp.task('bundle-common-js', function () {
    return gulp.src(
        [
            "Scripts/Common/*.js",
        ])
        .pipe(concat('common.js'))
        .pipe(gulp.dest('./wwwroot/library/js'));
});


//pages.js
gulp.task('bundle-pages-js', function () {
    return gulp.src(
        [
            "Scripts/Pages/*.js",
        ])
        .pipe(concat('pages.js'))
        .pipe(gulp.dest('./wwwroot/library/js'));
});

//all js
gulp.task('bundle-process-js', gulp.series(
    'bundle-common-js',
    'bundle-pages-js',
));