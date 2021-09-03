Imports System
Imports Microsoft.VisualBasic
Imports KryoSerializerInstance = org.apache.spark.serializer.KryoSerializerInstance
Imports SerializerInstance = org.apache.spark.serializer.SerializerInstance
Imports RecordReader = org.datavec.api.records.reader.RecordReader
Imports CSVRecordReader = org.datavec.api.records.reader.impl.csv.CSVRecordReader
Imports FileSplit = org.datavec.api.split.FileSplit
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports TagNames = org.nd4j.common.tests.tags.TagNames
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertFalse
import static org.junit.jupiter.api.Assertions.assertTrue

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

Namespace org.datavec.spark


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.FILE_IO) @Tag(TagNames.JAVA_ONLY) @Tag(TagNames.SPARK) @Tag(TagNames.DIST_SYSTEMS) public class TestKryoSerialization extends BaseSparkTest
	<Serializable>
	Public Class TestKryoSerialization
		Inherits BaseSparkTest

		Public Overrides Function useKryo() As Boolean
			Return True
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test public void testCsvRecordReader() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCsvRecordReader()
			Dim si As SerializerInstance = sc.env().serializer().newInstance()
			assertTrue(TypeOf si Is KryoSerializerInstance)

			Dim r1 As RecordReader = New CSVRecordReader(1,ControlChars.Tab)
			Dim r2 As RecordReader = serDe(r1, si)

			Dim f As File = (New ClassPathResource("iris_tab_delim.txt")).File
			r1.initialize(New org.datavec.api.Split.FileSplit(f))
			r2.initialize(New org.datavec.api.Split.FileSplit(f))

			Do While r1.hasNext()
				assertEquals(r1.next(), r2.next())
			Loop
			assertFalse(r2.hasNext())
		End Sub


		Private Function serDe(Of T)(ByVal [in] As T, ByVal si As SerializerInstance) As T
			Dim bb As ByteBuffer = si.serialize([in], Nothing)
			Return CType(si.deserialize(bb, Nothing), T)
		End Function
	End Class

End Namespace