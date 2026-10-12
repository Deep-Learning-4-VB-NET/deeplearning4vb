Imports System
Imports System.Collections.Generic
Imports System.Text
Imports Microsoft.VisualBasic
Imports IOUtils = org.apache.commons.io.IOUtils
Imports Configuration = org.apache.hadoop.conf.Configuration
Imports FileSystem = org.apache.hadoop.fs.FileSystem
Imports Path = org.apache.hadoop.fs.Path
Imports CompressionCodec = org.apache.hadoop.io.compress.CompressionCodec
Imports SparkConf = org.apache.spark.SparkConf
Imports SparkContext = org.apache.spark.SparkContext
Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports JavaSparkContext = org.apache.spark.api.java.JavaSparkContext
Imports DataAnalysis = org.datavec.api.transform.analysis.DataAnalysis
Imports Schema = org.datavec.api.transform.schema.Schema
Imports RandomSplit = org.datavec.api.transform.split.RandomSplit
Imports SplitStrategy = org.datavec.api.transform.split.SplitStrategy
Imports HtmlAnalysis = org.datavec.api.transform.ui.HtmlAnalysis
Imports org.datavec.api.writable

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.datavec.spark.transform.utils


	Public Class SparkUtils

		Public Shared Function splitData(Of T)(ByVal splitStrategy As SplitStrategy, ByVal data As JavaRDD(Of T), ByVal seed As Long) As IList(Of JavaRDD(Of T))

			If TypeOf splitStrategy Is org.datavec.api.transform.Split.RandomSplit Then

				Dim rs As org.datavec.api.transform.Split.RandomSplit = DirectCast(splitStrategy, org.datavec.api.transform.Split.RandomSplit)

				Dim fractionTrain As Double = rs.getFractionTrain()

				Dim splits() As Double = {fractionTrain, 1.0 - fractionTrain}

				Dim split() As JavaRDD(Of T) = data.randomSplit(splits, seed)
				Dim list As IList(Of JavaRDD(Of T)) = New List(Of JavaRDD(Of T))(2)
				Collections.addAll(list, split)

				Return list

			Else
				Throw New Exception("Not yet implemented")
			End If
		End Function

		''' <summary>
		''' Write a String to a file (on HDFS or local) in UTF-8 format
		''' </summary>
		''' <param name="path">       Path to write to </param>
		''' <param name="toWrite">    String to write </param>
		''' <param name="sc">         Spark context </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void writeStringToFile(String path, String toWrite, org.apache.spark.api.java.JavaSparkContext sc) throws IOException
		Public Shared Sub writeStringToFile(ByVal path As String, ByVal toWrite As String, ByVal sc As JavaSparkContext)
			writeStringToFile(path, toWrite, sc.sc())
		End Sub

		''' <summary>
		''' Write a String to a file (on HDFS or local) in UTF-8 format
		''' </summary>
		''' <param name="path">       Path to write to </param>
		''' <param name="toWrite">    String to write </param>
		''' <param name="sc">         Spark context </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void writeStringToFile(String path, String toWrite, org.apache.spark.SparkContext sc) throws IOException
		Public Shared Sub writeStringToFile(ByVal path As String, ByVal toWrite As String, ByVal sc As SparkContext)
			writeStringToFile(path, toWrite, sc.hadoopConfiguration())
		End Sub

		''' <summary>
		''' Write a String to a file (on HDFS or local) in UTF-8 format
		''' </summary>
		''' <param name="path">         Path to write to </param>
		''' <param name="toWrite">      String to write </param>
		''' <param name="hadoopConfig"> Hadoop configuration, for example from SparkContext.hadoopConfiguration() </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void writeStringToFile(String path, String toWrite, org.apache.hadoop.conf.Configuration hadoopConfig) throws IOException
		Public Shared Sub writeStringToFile(ByVal path As String, ByVal toWrite As String, ByVal hadoopConfig As Configuration)
			Dim fileSystem As FileSystem = FileSystem.get(hadoopConfig)
			Using bos As New BufferedOutputStream(fileSystem.create(New org.apache.hadoop.fs.Path(path)))
				bos.write(toWrite.GetBytes(Encoding.UTF8))
			End Using
		End Sub

		''' <summary>
		''' Read a UTF-8 format String from HDFS (or local)
		''' </summary>
		''' <param name="path">    Path to write the string </param>
		''' <param name="sc">      Spark context </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static String readStringFromFile(String path, org.apache.spark.api.java.JavaSparkContext sc) throws IOException
		Public Shared Function readStringFromFile(ByVal path As String, ByVal sc As JavaSparkContext) As String
			Return readStringFromFile(path, sc.sc())
		End Function

		''' <summary>
		''' Read a UTF-8 format String from HDFS (or local)
		''' </summary>
		''' <param name="path">    Path to write the string </param>
		''' <param name="sc">      Spark context </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static String readStringFromFile(String path, org.apache.spark.SparkContext sc) throws IOException
		Public Shared Function readStringFromFile(ByVal path As String, ByVal sc As SparkContext) As String
			Return readStringFromFile(path, sc.hadoopConfiguration())
		End Function

		''' <summary>
		''' Read a UTF-8 format String from HDFS (or local)
		''' </summary>
		''' <param name="path">         Path to write the string </param>
		''' <param name="hadoopConfig"> Hadoop configuration, for example from SparkContext.hadoopConfiguration() </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static String readStringFromFile(String path, org.apache.hadoop.conf.Configuration hadoopConfig) throws IOException
		Public Shared Function readStringFromFile(ByVal path As String, ByVal hadoopConfig As Configuration) As String
			Dim fileSystem As FileSystem = FileSystem.get(hadoopConfig)
			Using bis As New BufferedInputStream(fileSystem.open(New org.apache.hadoop.fs.Path(path)))
				Dim asBytes() As SByte = IOUtils.toByteArray(bis)
				Return StringHelper.NewString(asBytes, StandardCharsets.UTF_8)
			End Using
		End Function

		''' <summary>
		''' Write an object to HDFS (or local) using default Java object serialization
		''' </summary>
		''' <param name="path">       Path to write the object to </param>
		''' <param name="toWrite">    Object to write </param>
		''' <param name="sc">         Spark context </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void writeObjectToFile(String path, Object toWrite, org.apache.spark.api.java.JavaSparkContext sc) throws IOException
		Public Shared Sub writeObjectToFile(ByVal path As String, ByVal toWrite As Object, ByVal sc As JavaSparkContext)
			writeObjectToFile(path, toWrite, sc.sc())
		End Sub

		''' <summary>
		''' Write an object to HDFS (or local) using default Java object serialization
		''' </summary>
		''' <param name="path">       Path to write the object to </param>
		''' <param name="toWrite">    Object to write </param>
		''' <param name="sc">         Spark context </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void writeObjectToFile(String path, Object toWrite, org.apache.spark.SparkContext sc) throws IOException
		Public Shared Sub writeObjectToFile(ByVal path As String, ByVal toWrite As Object, ByVal sc As SparkContext)
			writeObjectToFile(path, toWrite, sc.hadoopConfiguration())
		End Sub

		''' <summary>
		''' Write an object to HDFS (or local) using default Java object serialization
		''' </summary>
		''' <param name="path">       Path to write the object to </param>
		''' <param name="toWrite">    Object to write </param>
		''' <param name="hadoopConfig"> Hadoop configuration, for example from SparkContext.hadoopConfiguration() </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void writeObjectToFile(String path, Object toWrite, org.apache.hadoop.conf.Configuration hadoopConfig) throws IOException
		Public Shared Sub writeObjectToFile(ByVal path As String, ByVal toWrite As Object, ByVal hadoopConfig As Configuration)
			Dim fileSystem As FileSystem = FileSystem.get(hadoopConfig)
			Using bos As New BufferedOutputStream(fileSystem.create(New org.apache.hadoop.fs.Path(path)))
				Dim oos As New ObjectOutputStream(bos)
				oos.writeObject(toWrite)
			End Using
		End Sub

		''' <summary>
		''' Read an object from HDFS (or local) using default Java object serialization
		''' </summary>
		''' <param name="path">    File to read </param>
		''' <param name="type">    Class of the object to read </param>
		''' <param name="sc">      Spark context </param>
		''' @param <T>     Type of the object to read </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static <T> T readObjectFromFile(String path, @Class<T> type, org.apache.spark.api.java.JavaSparkContext sc) throws IOException
		Public Shared Function readObjectFromFile(Of T)(ByVal path As String, ByVal type As Type(Of T), ByVal sc As JavaSparkContext) As T
			Return readObjectFromFile(path, type, sc.sc())
		End Function

		''' <summary>
		''' Read an object from HDFS (or local) using default Java object serialization
		''' </summary>
		''' <param name="path">    File to read </param>
		''' <param name="type">    Class of the object to read </param>
		''' <param name="sc">      Spark context </param>
		''' @param <T>     Type of the object to read </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static <T> T readObjectFromFile(String path, @Class<T> type, org.apache.spark.SparkContext sc) throws IOException
		Public Shared Function readObjectFromFile(Of T)(ByVal path As String, ByVal type As Type(Of T), ByVal sc As SparkContext) As T
			Return readObjectFromFile(path, type, sc.hadoopConfiguration())
		End Function

		''' <summary>
		''' Read an object from HDFS (or local) using default Java object serialization
		''' </summary>
		''' <param name="path">         File to read </param>
		''' <param name="type">         Class of the object to read </param>
		''' <param name="hadoopConfig"> Hadoop configuration, for example from SparkContext.hadoopConfiguration() </param>
		''' @param <T>          Type of the object to read </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static <T> T readObjectFromFile(String path, @Class<T> type, org.apache.hadoop.conf.Configuration hadoopConfig) throws IOException
		Public Shared Function readObjectFromFile(Of T)(ByVal path As String, ByVal type As Type(Of T), ByVal hadoopConfig As Configuration) As T
			Dim fileSystem As FileSystem = FileSystem.get(hadoopConfig)
			Using ois As New ObjectInputStream(New BufferedInputStream(fileSystem.open(New org.apache.hadoop.fs.Path(path))))
				Dim o As Object
				Try
					o = ois.readObject()
				Catch e As ClassNotFoundException
					Throw New Exception(e)
				End Try

				Return DirectCast(o, T)
			End Using
		End Function

		''' <summary>
		''' Write a schema to a HDFS (or, local) file in a human-readable format
		''' </summary>
		''' <param name="outputPath">    Output path to write to </param>
		''' <param name="schema">        Schema to write </param>
		''' <param name="sc">            Spark context </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void writeSchema(String outputPath, org.datavec.api.transform.schema.Schema schema, org.apache.spark.api.java.JavaSparkContext sc) throws IOException
		Public Shared Sub writeSchema(ByVal outputPath As String, ByVal schema As Schema, ByVal sc As JavaSparkContext)
			writeStringToFile(outputPath, schema.ToString(), sc)
		End Sub

		''' <summary>
		''' Write a DataAnalysis to HDFS (or locally) as a HTML file
		''' </summary>
		''' <param name="outputPath">      Output path </param>
		''' <param name="dataAnalysis">    Analysis to generate HTML file for </param>
		''' <param name="sc">              Spark context </param>
		Public Shared Sub writeAnalysisHTMLToFile(ByVal outputPath As String, ByVal dataAnalysis As DataAnalysis, ByVal sc As JavaSparkContext)
			Try
				Dim analysisAsHtml As String = HtmlAnalysis.createHtmlAnalysisString(dataAnalysis)
				writeStringToFile(outputPath, analysisAsHtml, sc)
			Catch e As Exception
				Throw New Exception("Error generating or writing HTML analysis file (normalized data)", e)
			End Try
		End Sub

		''' <summary>
		''' Wlite a set of writables (or, sequence) to HDFS (or, locally).
		''' </summary>
		''' <param name="outputPath">    Path to write the outptu </param>
		''' <param name="delim">         Delimiter </param>
		''' <param name="writables">     data to write </param>
		''' <param name="sc">            Spark context </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public static void writeWritablesToFile(String outputPath, String delim, java.util.List<java.util.List<Writable>> writables, org.apache.spark.api.java.JavaSparkContext sc) throws IOException
		Public Shared Sub writeWritablesToFile(ByVal outputPath As String, ByVal delim As String, ByVal writables As IList(Of IList(Of Writable)), ByVal sc As JavaSparkContext)
			Dim sb As New StringBuilder()
			For Each list As IList(Of Writable) In writables
				Dim first As Boolean = True
				For Each w As Writable In list
					If Not first Then
						sb.Append(delim)
					End If
					sb.Append(w.ToString())
					first = False
				Next w
				sb.Append(vbLf)
			Next list
			writeStringToFile(outputPath, sb.ToString(), sc)
		End Sub

		''' <summary>
		''' Register the DataVec writable classes for Kryo
		''' </summary>
		Public Shared Sub registerKryoClasses(ByVal conf As SparkConf)
			Dim classes As IList(Of Type) = New List(Of Type) From {Of Type}

			conf.registerKryoClasses(CType(classes.ToArray(), Type()))
		End Sub

		Public Shared Function getCompressionCodeClass(ByVal compressionCodecClass As String) As Type
			Dim tempClass As Type
			Try
				tempClass = Type.GetType(compressionCodecClass)
			Catch e As ClassNotFoundException
				Throw New Exception("Invalid class for compression codec: " & compressionCodecClass & " (not found)", e)
			End Try
			If Not (tempClass.IsAssignableFrom(GetType(CompressionCodec))) Then
				Throw New Exception("Invalid class for compression codec: " & compressionCodecClass & " (not a CompressionCodec)")
			End If
			Return CType(tempClass, Type)
		End Function
	End Class

End Namespace