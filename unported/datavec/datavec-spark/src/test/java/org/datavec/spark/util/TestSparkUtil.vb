Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Platform = com.sun.jna.Platform
Imports IOUtils = org.apache.commons.io.IOUtils
Imports DoubleWritable = org.datavec.api.writable.DoubleWritable
Imports IntWritable = org.datavec.api.writable.IntWritable
Imports Text = org.datavec.api.writable.Text
Imports Writable = org.datavec.api.writable.Writable
Imports BaseSparkTest = org.datavec.spark.BaseSparkTest
Imports SparkUtils = org.datavec.spark.transform.utils.SparkUtils
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TagNames = org.nd4j.common.tests.tags.TagNames
import static org.junit.jupiter.api.Assertions.assertEquals

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


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.SPARK) @Tag(TagNames.FILE_IO) @Tag(TagNames.DIST_SYSTEMS) public class TestSparkUtil extends org.datavec.spark.BaseSparkTest
	<Serializable>
	Public Class TestSparkUtil
		Inherits BaseSparkTest

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testWriteWritablesToFile() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testWriteWritablesToFile()
		   If Platform.isWindows() Then
			   Return
		   End If
			Dim l As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
			l.Add(New List(Of Writable) From {
				New Text("abc"),
				New DoubleWritable(2.0),
				New IntWritable(-1)
			})
			l.Add(New List(Of Writable) From {
				New Text("def"),
				New DoubleWritable(4.0),
				New IntWritable(-2)
			})

			Dim f As File = File.createTempFile("testSparkUtil", "txt")
			f.deleteOnExit()

			SparkUtils.writeWritablesToFile(f.getAbsolutePath(), ",", l, sc)

			Dim lines As IList(Of String) = IOUtils.readLines(New FileStream(f, FileMode.Open, FileAccess.Read))
			Dim expected As IList(Of String) = New List(Of String) From {"abc,2.0,-1", "def,4.0,-2"}

			assertEquals(expected, lines)

		End Sub

	End Class

End Namespace