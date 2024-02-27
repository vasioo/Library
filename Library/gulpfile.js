/*CSS and JS bundling*/

var gulp = require("gulp"),
    less = require("gulp-less"),
    concat = require("gulp-concat"),
    lessToVars = require('gulp-less-vars-to-js'),
    rename = require('gulp-rename'),
    replace = require('gulp-replace');


const autoprefixer = require('gulp-autoprefixer');

function compileSass() {
    return gulp.src('./wwwroot/library/scss/*.scss')
        .pipe(sass().on('error', sass.logError))
        .pipe(autoprefixer())
        .pipe(gulp.dest('.wwwroot/library/css/custom'));
}
exports.default = gulp.series(compileSass);

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