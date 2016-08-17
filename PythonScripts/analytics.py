


studentNames = ["Shubham", "Sakshi", "Dhvimidh", "Yug", "Dhruvi", "Arya", "Kashish", "Gitika", "Janmay", "Amoli", "Khushi", "Kushal", "Vivaan", "Chaitya", "Nirvi", "Sherman", "Shlok", "Swayam", "Viha", "Diya", "Palash", "Rishi", "Stuti", "Suhani"]
studentIDs = ["Y11sa01", "Y11sa02", "Y11sa03", "Y11sa04", "Y11sa05", "Y11sa06", "Y11sa07", "Y11sa08", "Y11sa09", "Y11sa10", "Y11sa11", "Y11sa12", "Y11sa13", "Y11sa14", "Y11sa15", "Y11sa16", "Y11sa17", "Y11sa18", "Y11sa19", "Y11sa20","Y11sa21","Y11sa22","Y11sa23","Y11sa24"]
print("Name, , Total, , , Simple Interest, , , Ratio and Proportion, , , Algebra, , , Mensuration, ");
print(" ,Attempts, Correct, Accuracy, Attempts, Correct, Accuracy, Attempts, Correct, Accuracy, Attempts, Correct, Accuracy, Attempts, Correct, Accuracy");
for i in range(len(studentNames)):
	#print (studentNames[i] + " "  + studentIDs[i])
	# print(studentNames[i])

	attempts = 0
	correct = 0

	attemptALG = 0
	attemptSI = 0
	attemptMEN = 0
	attemptRAP = 0

	correctALG = 0
	correctSI = 0
	correctMEN = 0
	correctRAP = 0

	fo = open("Analytics.csv", "r")
	for line in fo:
	    words = line.split(",")
	    t = ""
	    if(words[0] == studentIDs[i]):
	    	
	    	if(words[1][:3] == "MME"):
	    		attempts = attempts + 1
	    		attemptMEN = attemptMEN + 1
	    		t = "men"
	    	elif(words[1][:3] == "MSI"):
	    		attempts = attempts + 1
	    		attemptSI= attemptSI + 1
	    		t = "si"
	    	elif(words[1][:3] == "MAl"):
	    		attempts = attempts + 1
	    		attemptALG = attemptALG + 1;
	    		t = "alg"
	    	elif(words[1][:3] == "MRA"):
	    		attempts = attempts + 1
	    		attemptRAP = attemptRAP + 1;
	    		t = "rap"
	    	#else:
	    		#print(words[1][:3])
	    		
	        if(words[2] == "True"):
	            correct = correct + 1;
	            if(t == "men"):
	            	correctMEN = correctMEN + 1;
	            if(t == "alg"):
	            	correctALG = correctALG + 1;
	            if(t == "si"):
	            	correctSI = correctSI + 1;
	            if(t == "rap"):
	            	correctRAP = correctRAP + 1;
	            #if(t == ""):
	            #	print "T was empty"
		        
	attempted = attempts
	if(attempted == 0):
		print("ZERO")
		
	percCorrect = 100 * correct / attempted
	wrong = attempted - correct

	percMEN = 100 * correctMEN/attemptMEN;
	percSI = 100 * correctSI/attemptSI;
	percALG = 100 * correctALG/attemptALG;
	percRAP = 0;
	if(attemptRAP != 0):
		percRAP = 100 * correctRAP/attemptRAP;	

	percMENA = 100 * attemptMEN/attempted;
	percSIA = 100 * attemptSI/attempted;
	percALGA = 100 * attemptALG/attempted;
	percRAPA = 100 * attemptRAP/attempted;

	#print ("STATS FOR " + studentNames[i] + " id: " + studentIDs[i]);
	
	print(studentNames[i] + "," + str(attempted) + "," + str(correct) + "," + str(percCorrect) + "%," + str(attemptSI) + "," + str(correctSI) + "," + str(percSI) + "%,"  + str(attemptRAP) + "," + str(correctRAP) + "," + str(percRAP) + "%,"  + str(attemptALG) + "," + str(correctALG) + "," + str(percALG) + "%,"  + str(attemptMEN) + "," + str(correctMEN) + "," + str(percMEN) + "%" )

	# print ("Total Attempts " + str(attempted))

	# print("Mensuration Attempts: " + str(percMENA) + "% or " + str(attemptMEN) + " sums")
	# print("Algebra Attempts: " +  str(percALGA) + "% or "  + str(attemptALG)+ " sums")
	# print("Simple Interest Attempts: "  + str(percSIA) + "% or " + str(attemptSI)+ " sums")
	# print("Ratio and Proportion Attempts: "  + str(percRAPA) + "% or " + str(attemptRAP)+ " sums")

	# print("")
	# print ("Overall Accuracy" " " + str(percCorrect) + "% or " + str(correct) + " sums")

	# print("Mensuration Accuracy: "  + str(percMEN) + "% or " + str(correctMEN) + " sums")
	# print("Algebra Accuracy: "  + str(percALG) + "% or " + str(correctALG) + " sums")
	# print("Simple Interest Accuracy: " +str(percSI) + "% or " + str(correctSI) + " sums")
	# print("Ratio and Proportion Accuracy: " +str(percRAP) + "% or " + str(correctRAP) + " sums")
	# print ("")
	# print ("")
	