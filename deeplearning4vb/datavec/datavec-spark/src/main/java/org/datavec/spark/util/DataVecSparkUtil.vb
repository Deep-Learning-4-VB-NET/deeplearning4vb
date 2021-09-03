Imports Text = org.apache.hadoop.io.Text
Imports JavaPairRDD = org.apache.spark.api.java.JavaPairRDD
Imports JavaSparkContext = org.apache.spark.api.java.JavaSparkContext
Imports PortableDataStream = org.apache.spark.input.PortableDataStream
Imports BytesPairWritable = org.datavec.spark.functions.pairdata.BytesPairWritable
Imports MapToBytesPairWritableFunction = org.datavec.spark.functions.pairdata.MapToBytesPairWritableFunction
Imports PathToKeyConverter = org.datavec.spark.functions.pairdata.PathToKeyConverter
Imports PathToKeyFunction = org.datavec.spark.functions.pairdata.PathToKeyFunction
Imports Tuple3 = scala.Tuple3

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

Namespace org.datavec.spark.util

	Public Class DataVecSparkUtil

		''' <summary>
		'''Same as <seealso cref="combineFilesForSequenceFile(JavaSparkContext, String, String, PathToKeyConverter, PathToKeyConverter)"/>
		''' but with the PathToKeyConverter used for both file sources
		''' </summary>
		Public Shared Function combineFilesForSequenceFile(ByVal sc As JavaSparkContext, ByVal path1 As String, ByVal path2 As String, ByVal converter As PathToKeyConverter) As JavaPairRDD(Of Text, BytesPairWritable)
			Return combineFilesForSequenceFile(sc, path1, path2, converter, converter)
		End Function

		''' <summary>
		'''This is a convenience method to combine data from separate files together (intended to write to a sequence file, using
		''' <seealso cref="org.apache.spark.api.java.JavaPairRDD.saveAsNewAPIHadoopFile(String, Class, Class, Class) "/>)<br>
		''' A typical use case is to combine input and label data from different files, for later parsing by a RecordReader
		''' or SequenceRecordReader.
		''' A typical use case is as follows:<br>
		''' Given two paths (directories), combine the files in these two directories into pairs.<br>
		''' Then, for each pair of files, convert the file contents into a <seealso cref="BytesPairWritable"/>, which also contains
		''' the original file paths of the files.<br>
		''' The assumptions are as follows:<br>
		''' - For every file in the first directory, there is an equivalent file in the second directory (i.e., same key)<br>
		''' - The pairing of files can be done based on the paths of the files; paths are mapped to a key using a <seealso cref="PathToKeyConverter"/>;
		'''   keys are then matched to give pairs of files<br>
		''' <br><br>
		''' <b>Example usage</b>: to combine all files in directory {@code dir1} with equivalent files in {@code dir2}, by file name:
		''' <pre>
		''' <code>JavaSparkContext sc = ...;
		''' String path1 = "/dir1";
		''' String path2 = "/dir2";
		''' PathToKeyConverter pathConverter = new PathToKeyConverterFilename();
		''' JavaPairRDD&lt;Text,BytesPairWritable&gt; toWrite = DataVecSparkUtil.combineFilesForSequenceFile(sc, path1, path2, pathConverter, pathConverter );
		''' String outputPath = "/my/output/path";
		''' toWrite.saveAsNewAPIHadoopFile(outputPath, Text.class, BytesPairWritable.class, SequenceFileOutputFormat.class);
		''' </code>
		''' </pre>
		''' Result: the file contexts aggregated (pairwise), written to a hadoop sequence file at /my/output/path
		''' 
		''' </summary>
		''' <param name="sc"> Spark context </param>
		''' <param name="path1"> First directory (passed to JavaSparkContext.binaryFiles(path1)) </param>
		''' <param name="path2"> Second directory (passed to JavaSparkContext.binaryFiles(path1)) </param>
		''' <param name="converter1"> Converter, to convert file paths in first directory to a key (to allow files to be matched/paired by key) </param>
		''' <param name="converter2"> As above, for second directory
		''' @return </param>
		Public Shared Function combineFilesForSequenceFile(ByVal sc As JavaSparkContext, ByVal path1 As String, ByVal path2 As String, ByVal converter1 As PathToKeyConverter, ByVal converter2 As PathToKeyConverter) As JavaPairRDD(Of Text, BytesPairWritable)
			Dim first As JavaPairRDD(Of String, PortableDataStream) = sc.binaryFiles(path1)
			Dim second As JavaPairRDD(Of String, PortableDataStream) = sc.binaryFiles(path2)

			'Now: process keys (paths) so that they can be merged
			Dim first2 As JavaPairRDD(Of String, Tuple3(Of String, Integer, PortableDataStream)) = first.mapToPair(New PathToKeyFunction(0, converter1))
			Dim second2 As JavaPairRDD(Of String, Tuple3(Of String, Integer, PortableDataStream)) = second.mapToPair(New PathToKeyFunction(1, converter2))
			Dim merged As JavaPairRDD(Of String, Tuple3(Of String, Integer, PortableDataStream)) = first2.union(second2)

			'Combine into pairs, and prepare for writing
			Dim toWrite As JavaPairRDD(Of Text, BytesPairWritable) = merged.groupByKey().mapToPair(New MapToBytesPairWritableFunction())
			Return toWrite
		End Function

	End Class

End Namespace