
output:
	find Source/ | grep .cs | xargs mcs -out:Output/Bundlecat.cil

prep:
	mkdir Output/
	echo "cd Output;mono Bundlecat.cil;cd .." >> test.sh
	chmod 755 test.sh

run:
	./test.sh
