import rollup from 'rollup'
import nodeResolve from 'rollup-plugin-node-resolve'
import commonjs from 'rollup-plugin-commonjs';
import uglify from 'rollup-plugin-uglify'

export default {
    entry: '.build/public/boot-aot.js',
    dest: 'wwwroot/app-public.min.js', // output a single application bundle
    sourceMap: true,
    format: 'iife',
    moduleName: "PublicBundle",
    plugins: [
        nodeResolve({ jsnext: true, module: true }),
        commonjs({
            include: 'node_modules/**',
        }),
        uglify()
    ]
}