Imports System.Collections.Generic
Imports FileUtils = org.apache.commons.io.FileUtils

Namespace org.nd4j.fileupdater


	Public Interface FileUpdater

		Function patterns() As IDictionary(Of String, String)

'JAVA TO VB CONVERTER TODO TASK: There is no equivalent in VB to Java default interface methods:
'		default boolean pathMatches(java.io.File inputPath)
	'	{
	'		if(inputPath == Nothing)
	'			Return False;
	'		Return !inputPath.getParentFile().getName().equals("target") && inputPath.getName().equals("pom.xml");
	'	}


'JAVA TO VB CONVERTER TODO TASK: There is no equivalent in VB to Java default interface methods:
'		default void patternReplace(java.io.File inputFilePath) throws java.io.IOException
	'	{
	'		System.out.println("Updating " + inputFilePath);
	'		String content = FileUtils.readFileToString(inputFilePath, Charset.defaultCharset());
	'		String newContent = content;
	'		for(Map.Entry<String,String> patternEntry : patterns().entrySet())
	'		{
	'			newContent = newContent.replaceAll(patternEntry.getKey(),patternEntry.getValue());
	'		}
	'
	'		FileUtils.writeStringToFile(inputFilePath,newContent,Charset.defaultCharset(),False);
	'
	'	}




	End Interface

End Namespace