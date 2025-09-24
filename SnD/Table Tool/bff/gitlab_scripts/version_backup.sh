RE='[^0-9]*\([0-9]*\)[.]\([0-9]*\)[.]\([0-9]*\)\([0-9A-Za-z-]*\)'
REBads='[_]'

echo $CI_COMMIT_REF_NAME >> temp.tag
sed -i "s/[0-9]\+.[0-9]\+.[0-9]\+/true/" temp.tag
isTagged=$(cat temp.tag)
rm temp.tag
clearRef=$(echo "$CI_COMMIT_REF_NAME" | sed -e 's/_/-/g' )

if [[ $isTagged == "true" ]]
then
  # echo "tagged"
  base="$1"
  if [ -z "$1" ]
  then
    base=$(git tag --sort=version:refname 2>/dev/null| tail -n 1)
    if [ -z "$base" ]
    then
      base=0.0.0
    fi
  fi
  MAJOR=`echo $base | sed -e "s#$RE#\1#"`
  MINOR=`echo $base | sed -e "s#$RE#\2#"`
  PATCH=`echo $base | sed -e "s#$RE#\3#"`
  version="$MAJOR.$MINOR.$PATCH"
  # return the calculated tag
  echo "$version"
# else return the current tag
else
  # echo "not tagged branch"
  base="$1"
  if [ -z "$1" ]
  then
    base=$(git tag --sort=version:refname 2>/dev/null| tail -n 1)
    if [ -z "$base" ]
    then
      base=0.0.0
    fi
  fi
  MAJOR=`echo $base | sed -e "s#$RE#\1#"`
  MINOR=`echo $base | sed -e "s#$RE#\2#"`
  PATCH=`echo $base | sed -e "s#$RE#\3#"`
  version="$MAJOR.$MINOR.$PATCH"

  # remove / from build
  echo "${version}-${clearRef}" | sed 's/\//-/'
fi