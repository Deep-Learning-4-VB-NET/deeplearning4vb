Imports System
Imports System.Collections.Generic
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertFalse
import static org.junit.jupiter.api.Assertions.assertNotNull
import static org.junit.jupiter.api.Assertions.assertTrue
Imports DbUtils = org.apache.commons.dbutils.DbUtils
Imports EmbeddedDataSource = org.apache.derby.jdbc.EmbeddedDataSource
Imports Configuration = org.datavec.api.conf.Configuration
Imports Record = org.datavec.api.records.Record
Imports RecordListener = org.datavec.api.records.listener.RecordListener
Imports LogRecordListener = org.datavec.api.records.listener.impl.LogRecordListener
Imports RecordMetaData = org.datavec.api.records.metadata.RecordMetaData
Imports RecordMetaDataJdbc = org.datavec.jdbc.records.metadata.RecordMetaDataJdbc
Imports RecordMetaDataLine = org.datavec.api.records.metadata.RecordMetaDataLine
Imports JDBCRecordReader = org.datavec.jdbc.records.reader.impl.jdbc.JDBCRecordReader
Imports BooleanWritable = org.datavec.api.writable.BooleanWritable
Imports DoubleWritable = org.datavec.api.writable.DoubleWritable
Imports FloatWritable = org.datavec.api.writable.FloatWritable
Imports IntWritable = org.datavec.api.writable.IntWritable
Imports LongWritable = org.datavec.api.writable.LongWritable
Imports Text = org.datavec.api.writable.Text
Imports Writable = org.datavec.api.writable.Writable
Imports org.junit.jupiter.api
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith
Imports TagNames = org.nd4j.common.tests.tags.TagNames
import static org.junit.jupiter.api.Assertions.assertThrows

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
Namespace org.datavec.api.records.reader.impl


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Jdbc Record Reader Test") @Tag(TagNames.FILE_IO) @Tag(TagNames.JAVA_ONLY) public class JDBCRecordReaderTest
	Public Class JDBCRecordReaderTest


		Friend conn As Connection

		Friend dataSource As EmbeddedDataSource

		Private ReadOnly dbName As String = "datavecTests"

		Private ReadOnly driverClassName As String = "org.apache.derby.jdbc.EmbeddedDriver"

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach void setUp() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub setUp()
			dataSource = New EmbeddedDataSource()
			dataSource.setDatabaseName(dbName)
			dataSource.setCreateDatabase("create")
			conn = dataSource.getConnection()
			TestDb.dropTables(conn)
			TestDb.buildCoffeeTable(conn)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach void tearDown() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub tearDown()
			DbUtils.closeQuietly(conn)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Simple Iter") void testSimpleIter(@TempDir Path testDir) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testSimpleIter(ByVal testDir As Path)
			Dim f As File = testDir.resolve("new-folder").toFile()
			assertTrue(f.mkdirs())
			System.setProperty("derby.system.home", f.getAbsolutePath())
			Using reader As org.datavec.jdbc.records.reader.impl.jdbc.JDBCRecordReader = getInitializedReader("SELECT * FROM Coffee")
				Dim records As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
				Do While reader.hasNext()
					Dim values As IList(Of Writable) = reader.next()
					records.Add(values)
				Loop
				assertFalse(records.Count = 0)
				Dim first As IList(Of Writable) = records(0)
				assertEquals(New Text("Bolivian Dark"), first(0))
				assertEquals(New Text("14-001"), first(1))
				assertEquals(New DoubleWritable(8.95), first(2))
			End Using
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Simple With Listener") void testSimpleWithListener() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testSimpleWithListener()
			Using reader As org.datavec.jdbc.records.reader.impl.jdbc.JDBCRecordReader = getInitializedReader("SELECT * FROM Coffee")
				Dim recordListener As RecordListener = New LogRecordListener()
				reader.setListeners(recordListener)
				reader.next()
				assertTrue(recordListener.invoked())
			End Using
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Reset") void testReset() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testReset()
			Using reader As org.datavec.jdbc.records.reader.impl.jdbc.JDBCRecordReader = getInitializedReader("SELECT * FROM Coffee")
				Dim records As IList(Of IList(Of Writable)) = New List(Of IList(Of Writable))()
				records.Add(reader.next())
				reader.reset()
				records.Add(reader.next())
				assertEquals(2, records.Count)
				assertEquals(New Text("Bolivian Dark"), records(0)(0))
				assertEquals(New Text("Bolivian Dark"), records(1)(0))
			End Using
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Lacking Data Source Should Fail") void testLackingDataSourceShouldFail()
		Friend Overridable Sub testLackingDataSourceShouldFail()
			assertThrows(GetType(System.InvalidOperationException), Sub()
			Using reader As New org.datavec.jdbc.records.reader.impl.jdbc.JDBCRecordReader("SELECT * FROM Coffee")
				reader.initialize(Nothing)
			End Using
			End Sub)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Configuration Data Source Initialization") void testConfigurationDataSourceInitialization() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testConfigurationDataSourceInitialization()
			Using reader As New org.datavec.jdbc.records.reader.impl.jdbc.JDBCRecordReader("SELECT * FROM Coffee")
				Dim conf As New Configuration()
				conf.set(JDBCRecordReader.JDBC_URL, "jdbc:derby:" & dbName & ";create=true")
				conf.set(JDBCRecordReader.JDBC_DRIVER_CLASS_NAME, driverClassName)
				reader.initialize(conf, Nothing)
				assertTrue(reader.hasNext())
			End Using
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Init Configuration Missing Parameters Should Fail") void testInitConfigurationMissingParametersShouldFail()
		Friend Overridable Sub testInitConfigurationMissingParametersShouldFail()
			assertThrows(GetType(System.ArgumentException), Sub()
			Using reader As New org.datavec.jdbc.records.reader.impl.jdbc.JDBCRecordReader("SELECT * FROM Coffee")
				Dim conf As New Configuration()
				conf.set(JDBCRecordReader.JDBC_URL, "should fail anyway")
				reader.initialize(conf, Nothing)
			End Using
			End Sub)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Record Data Input Stream Should Fail") void testRecordDataInputStreamShouldFail()
		Friend Overridable Sub testRecordDataInputStreamShouldFail()
			assertThrows(GetType(System.NotSupportedException), Sub()
			Using reader As org.datavec.jdbc.records.reader.impl.jdbc.JDBCRecordReader = getInitializedReader("SELECT * FROM Coffee")
				reader.record(Nothing, Nothing)
			End Using
			End Sub)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Load From Meta Data") void testLoadFromMetaData() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testLoadFromMetaData()
			Using reader As org.datavec.jdbc.records.reader.impl.jdbc.JDBCRecordReader = getInitializedReader("SELECT * FROM Coffee")
				Dim rmd As New RecordMetaDataJdbc(New URI(conn.getMetaData().getURL()), "SELECT * FROM Coffee WHERE ProdNum = ?", Collections.singletonList("14-001"), reader.GetType())
				Dim res As Record = reader.loadFromMetaData(rmd)
				assertNotNull(res)
				assertEquals(New Text("Bolivian Dark"), res.getRecord()(0))
				assertEquals(New Text("14-001"), res.getRecord()(1))
				assertEquals(New DoubleWritable(8.95), res.getRecord()(2))
			End Using
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Next Record") void testNextRecord() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testNextRecord()
			Using reader As org.datavec.jdbc.records.reader.impl.jdbc.JDBCRecordReader = getInitializedReader("SELECT * FROM Coffee")
				Dim r As Record = reader.nextRecord()
				Dim fields As IList(Of Writable) = r.getRecord()
				Dim meta As RecordMetaData = r.MetaData
				assertNotNull(r)
				assertNotNull(fields)
				assertNotNull(meta)
				assertEquals(New Text("Bolivian Dark"), fields(0))
				assertEquals(New Text("14-001"), fields(1))
				assertEquals(New DoubleWritable(8.95), fields(2))
				assertEquals(GetType(RecordMetaDataJdbc), meta.GetType())
			End Using
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Next Record And Recover") void testNextRecordAndRecover() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testNextRecordAndRecover()
			Using reader As org.datavec.jdbc.records.reader.impl.jdbc.JDBCRecordReader = getInitializedReader("SELECT * FROM Coffee")
				Dim r As Record = reader.nextRecord()
				Dim fields As IList(Of Writable) = r.getRecord()
				Dim meta As RecordMetaData = r.MetaData
				Dim recovered As Record = reader.loadFromMetaData(meta)
				Dim fieldsRecovered As IList(Of Writable) = recovered.getRecord()
				assertEquals(fields.Count, fieldsRecovered.Count)
				For i As Integer = 0 To fields.Count - 1
					assertEquals(fields(i), fieldsRecovered(i))
				Next i
			End Using
		End Sub

		' Resetting the record reader when initialized as forward only should fail
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Reset Forward Only Should Fail") void testResetForwardOnlyShouldFail()
		Friend Overridable Sub testResetForwardOnlyShouldFail()
			assertThrows(GetType(Exception), Sub()
			Using reader As New org.datavec.jdbc.records.reader.impl.jdbc.JDBCRecordReader("SELECT * FROM Coffee", dataSource)
				Dim conf As New Configuration()
				conf.setInt(JDBCRecordReader.JDBC_RESULTSET_TYPE, ResultSet.TYPE_FORWARD_ONLY)
				reader.initialize(conf, Nothing)
				reader.next()
				reader.reset()
			End Using
			End Sub)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Read All Types") void testReadAllTypes() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub testReadAllTypes()
			TestDb.buildAllTypesTable(conn)
			Using reader As New org.datavec.jdbc.records.reader.impl.jdbc.JDBCRecordReader("SELECT * FROM AllTypes", dataSource)
				reader.initialize(Nothing)
				Dim item As IList(Of Writable) = reader.next()
				assertEquals(item.Count, 15)
				' boolean to boolean
				assertEquals(GetType(BooleanWritable), item(0).GetType())
				' date to text
				assertEquals(GetType(Text), item(1).GetType())
				' time to text
				assertEquals(GetType(Text), item(2).GetType())
				' timestamp to text
				assertEquals(GetType(Text), item(3).GetType())
				' char to text
				assertEquals(GetType(Text), item(4).GetType())
				' long varchar to text
				assertEquals(GetType(Text), item(5).GetType())
				' varchar to text
				assertEquals(GetType(Text), item(6).GetType())
				assertEquals(GetType(DoubleWritable), item(7).GetType())
				' real to float
				assertEquals(GetType(FloatWritable), item(8).GetType())
				' decimal to double
				assertEquals(GetType(DoubleWritable), item(9).GetType())
				' numeric to double
				assertEquals(GetType(DoubleWritable), item(10).GetType())
				' double to double
				assertEquals(GetType(DoubleWritable), item(11).GetType())
				' integer to integer
				assertEquals(GetType(IntWritable), item(12).GetType())
				' small int to integer
				assertEquals(GetType(IntWritable), item(13).GetType())
				' bigint to long
				assertEquals(GetType(LongWritable), item(14).GetType())
			End Using
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Next No More Should Fail") void testNextNoMoreShouldFail()
		Friend Overridable Sub testNextNoMoreShouldFail()
			assertThrows(GetType(Exception), Sub()
			Using reader As org.datavec.jdbc.records.reader.impl.jdbc.JDBCRecordReader = getInitializedReader("SELECT * FROM Coffee")
				Do While reader.hasNext()
					reader.next()
				Loop
				reader.next()
			End Using
			End Sub)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test Invalid Metadata Should Fail") void testInvalidMetadataShouldFail()
		Friend Overridable Sub testInvalidMetadataShouldFail()
			assertThrows(GetType(System.ArgumentException), Sub()
			Using reader As org.datavec.jdbc.records.reader.impl.jdbc.JDBCRecordReader = getInitializedReader("SELECT * FROM Coffee")
				Dim md As New RecordMetaDataLine(1, New URI("file://test"), GetType(JDBCRecordReader))
				reader.loadFromMetaData(md)
			End Using
			End Sub)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private org.datavec.jdbc.records.reader.impl.jdbc.JDBCRecordReader getInitializedReader(String query) throws Exception
		Private Function getInitializedReader(ByVal query As String) As JDBCRecordReader
			' ProdNum column
			Dim indices() As Integer = { 1 }
			Dim reader As New JDBCRecordReader(query, dataSource, "SELECT * FROM Coffee WHERE ProdNum = ?", indices)
			reader.setTrimStrings(True)
			reader.initialize(Nothing)
			Return reader
		End Function
	End Class

End Namespace