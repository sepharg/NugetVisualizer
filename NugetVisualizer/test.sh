 cd $1
 find -type d -printf '%d\t%P\n' | sort -r -nk1 | cut -f2-