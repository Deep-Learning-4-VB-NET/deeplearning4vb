Imports System
Imports System.Collections.Generic
Imports DriverDataSource = com.zaxxer.hikari.util.DriverDataSource
Imports Setter = lombok.Setter
Imports DbUtils = org.apache.commons.dbutils.DbUtils
Imports QueryRunner = org.apache.commons.dbutils.QueryRunner
Imports ArrayHandler = org.apache.commons.dbutils.handlers.ArrayHandler
Imports Configuration = org.datavec.api.conf.Configuration
Imports Record = org.datavec.api.records.Record
Imports RecordMetaData = org.datavec.api.records.metadata.RecordMetaData
Imports RecordMetaDataJdbc = org.datavec.jdbc.records.metadata.RecordMetaDataJdbc
Imports BaseRecordReader = org.datavec.api.records.reader.BaseRecordReader
Imports InputSplit = org.datavec.api.split.InputSplit
Imports JdbcWritableConverter = org.datavec.jdbc.util.JdbcWritableConverter
Imports ResettableResultSetIterator = org.datavec.jdbc.util.ResettableResultSetIterator
Imports Writable = org.datavec.api.writable.Writable

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

Namespace org.datavec.jdbc.records.reader.impl.jdbc

	<Serializable>
	Public Class JDBCRecordReader
		Inherits BaseRecordReader

		Private ReadOnly query As String
		Private conn As Connection
		Private statement As Statement
		Private iter As ResettableResultSetIterator
		Private meta As ResultSetMetaData
		Private configuration As Configuration
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter private boolean trimStrings = false;
		Private trimStrings As Boolean = False
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter private int resultSetType = java.sql.ResultSet.TYPE_SCROLL_INSENSITIVE;
		Private resultSetType As Integer = ResultSet.TYPE_SCROLL_INSENSITIVE
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Setter private javax.sql.DataSource dataSource;
		Private dataSource As DataSource
		Private ReadOnly metadataQuery As String
		Private ReadOnly metadataIndices() As Integer

		Public Shared ReadOnly TRIM_STRINGS As String = NAME_SPACE & ".trimStrings"
		Public Shared ReadOnly JDBC_URL As String = NAME_SPACE & ".jdbcUrl"
		Public Shared ReadOnly JDBC_DRIVER_CLASS_NAME As String = NAME_SPACE & ".jdbcDriverClassName"
		Public Shared ReadOnly JDBC_USERNAME As String = NAME_SPACE & ".jdbcUsername"
		Public Shared ReadOnly JDBC_PASSWORD As String = NAME_SPACE & ".jdbcPassword"
		Public Shared ReadOnly JDBC_RESULTSET_TYPE As String = NAME_SPACE & ".resultSetType"

		''' <summary>
		''' Build a new JDBCRecordReader with a given query. After constructing the reader in this way, the initialize method
		''' must be called and provided with configuration values for the datasource initialization.
		''' </summary>
		''' <param name="query"> Query to execute and on which the reader will iterate. </param>
		Public Sub New(ByVal query As String)
			Me.query = query
			Me.metadataQuery = Nothing
			Me.metadataIndices = Nothing
		End Sub

		''' <summary>
		''' Build a new JDBCRecordReader with a given query. If initialize is called with configuration values set for
		''' datasource initialization, the datasource provided to this constructor will be overriden.
		''' </summary>
		''' <param name="query"> Query to execute and on which the reader will iterate. </param>
		''' <param name="dataSource"> Initialized DataSource to use for iteration </param>
		Public Sub New(ByVal query As String, ByVal dataSource As DataSource)
			Me.query = query
			Me.dataSource = dataSource
			Me.metadataQuery = Nothing
			Me.metadataIndices = Nothing
		End Sub

		''' <summary>
		''' Same as JDBCRecordReader(String query, DataSource dataSource) but also provides a query and column indices to use
		''' for saving metadata (see <seealso cref="loadFromMetaData(RecordMetaData)"/>)
		''' </summary>
		''' <param name="query"> Query to execute and on which the reader will iterate. </param>
		''' <param name="dataSource"> Initialized DataSource to use for iteration. </param>
		''' <param name="metadataQuery"> Query to execute when recovering a single record from metadata </param>
		''' <param name="metadataIndices"> Column indices of which values will be saved in each record's metadata </param>
		Public Sub New(ByVal query As String, ByVal dataSource As DataSource, ByVal metadataQuery As String, ByVal metadataIndices() As Integer)
			Me.query = query
			Me.dataSource = dataSource
			Me.metadataQuery = metadataQuery
			Me.metadataIndices = metadataIndices
		End Sub

		''' <summary>
		''' Initialize all required jdbc elements and make the reader ready for iteration.
		''' </summary>
		''' <param name="split"> not handled yet, will be discarded </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void initialize(org.datavec.api.split.InputSplit split) throws IOException, InterruptedException
		Public Overrides Sub initialize(ByVal split As InputSplit)
			If dataSource Is Nothing Then
				Throw New System.InvalidOperationException("Cannot initialize : no datasource")
			End If
			initializeJdbc()
		End Sub

		''' <summary>
		''' Initialize all required jdbc elements and make the reader ready for iteration.
		''' 
		''' Possible configuration keys :
		''' <ol>
		'''     <li>JDBCRecordReader.TRIM_STRINGS : Whether or not read strings should be trimmed before being returned. False by default</li>
		'''     <li>JDBCRecordReader.JDBC_URL : Jdbc url to use for datastource configuration (see JDBCRecordReaderTest for examples)</li>
		'''     <li>JDBCRecordReader.JDBC_DRIVER_CLASS_NAME : Driver class to use for datasource configuration</li>
		'''     <li>JDBCRecordReader.JDBC_USERNAME && JDBC_PASSWORD : Username and password to use for datasource configuration</li>
		'''     <li>JDBCRecordReader.JDBC_RESULTSET_TYPE : ResultSet type to use (int value defined in jdbc doc)</li>
		''' </ol>
		''' 
		''' Url and driver class name are not mandatory. If one of them is specified, the other must be specified as well. If
		''' they are set and there already is a DataSource set in the reader, it will be discarded and replaced with the
		''' newly created one.
		''' </summary>
		''' <param name="conf"> a configuration for initialization </param>
		''' <param name="split"> not handled yet, will be discarded </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void initialize(org.datavec.api.conf.Configuration conf, org.datavec.api.split.InputSplit split) throws IOException, InterruptedException
		Public Overrides Sub initialize(ByVal conf As Configuration, ByVal split As InputSplit)
			Me.Conf = conf
			Me.setTrimStrings(conf.getBoolean(TRIM_STRINGS, trimStrings))
			Me.setResultSetType(conf.getInt(JDBC_RESULTSET_TYPE, resultSetType))

			Dim jdbcUrl As String = conf.get(JDBC_URL)
			Dim driverClassName As String = conf.get(JDBC_DRIVER_CLASS_NAME)
			' url and driver must be both unset or both present
			If jdbcUrl Is Nothing Xor driverClassName Is Nothing Then
				Throw New System.ArgumentException("Both jdbc url and driver class name must be provided in order to configure JDBCRecordReader's datasource")
			' Both set, initialiaze the datasource
			ElseIf jdbcUrl IsNot Nothing Then
				' FIXME : find a way to read wildcard properties from conf in order to fill the third argument bellow
				Me.dataSource = New DriverDataSource(jdbcUrl, driverClassName, New Properties(), conf.get(JDBC_USERNAME), conf.get(JDBC_PASSWORD))
			End If
			Me.initializeJdbc()
		End Sub

		Private Sub initializeJdbc()
			Try
				Me.conn = dataSource.getConnection()
				Me.statement = conn.createStatement(Me.resultSetType, ResultSet.CONCUR_READ_ONLY)
				Me.statement.closeOnCompletion()
				Dim rs As ResultSet = statement.executeQuery(Me.query)
				Me.meta = rs.getMetaData()
				Me.iter = New ResettableResultSetIterator(rs)
			Catch e As SQLException
				closeJdbc()
				Throw New Exception("Could not connect to the database", e)
			End Try
		End Sub

		Public Overrides Function [next]() As IList(Of Writable)
'JAVA TO VB CONVERTER NOTE: The local variable next was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim next_Conflict() As Object = iter.next()
			invokeListeners(next_Conflict)
			Return toWritable(next_Conflict)
		End Function

		Private Function toWritable(ByVal item() As Object) As IList(Of Writable)
			Dim ret As IList(Of Writable) = New List(Of Writable)()
			invokeListeners(item)
			For i As Integer = 0 To item.Length - 1
				Try
					Dim columnValue As Object = item(i)
					If trimStrings AndAlso TypeOf columnValue Is String Then
						columnValue = DirectCast(columnValue, String).Trim()
					End If
					' Note, getColumnType first argument is column number starting from 1
					Dim writable As Writable = JdbcWritableConverter.convert(columnValue, meta.getColumnType(i + 1))
					ret.Add(writable)
				Catch e As SQLException
					closeJdbc()
					Throw New Exception("Error reading database metadata")
				End Try
			Next i

			Return ret
		End Function

		Public Overrides Function hasNext() As Boolean
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Return iter.hasNext()
		End Function

		Public Overrides ReadOnly Property Labels As IList(Of String)
			Get
				Throw New System.NotSupportedException("JDBCRecordReader does not support getLabels yet")
			End Get
		End Property

		''' <summary>
		''' Depending on the jdbc driver implementation, this will probably fail if the resultset was created with ResultSet.TYPE_FORWARD_ONLY
		''' </summary>
		Public Overrides Sub reset()
			iter.reset()
		End Sub

		Public Overrides Function resetSupported() As Boolean
			Return True
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<org.datavec.api.writable.Writable> record(java.net.URI uri, java.io.DataInputStream dataInputStream) throws java.io.IOException
		Public Overrides Function record(ByVal uri As URI, ByVal dataInputStream As DataInputStream) As IList(Of Writable)
			Throw New System.NotSupportedException("JDBCRecordReader does not support reading from a DataInputStream")
		End Function

		''' <summary>
		''' Get next record with metadata. See <seealso cref="loadFromMetaData(RecordMetaData)"/> for details on metadata structure.
		''' </summary>
		Public Overrides Function nextRecord() As Record
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Dim [next]() As Object = iter.next()
			invokeListeners([next])

			Dim location As URI
			Try
				location = New URI(conn.getMetaData().getURL())
			Catch e As Exception When TypeOf e Is SQLException OrElse TypeOf e Is URISyntaxException
				Throw New System.InvalidOperationException("Could not get sql connection metadata", e)
			End Try

			Dim params As IList(Of Object) = New List(Of Object)()
			If metadataIndices IsNot Nothing Then
				For Each index As Integer In metadataIndices
					params.Add([next](index))
				Next index
			End If
			Dim rmd As New RecordMetaDataJdbc(location, Me.metadataQuery, params, Me.GetType())

			Return New org.datavec.api.records.impl.Record(toWritable([next]), rmd)
		End Function

		''' <summary>
		''' Record metadata for this reader consist in two elements :<br />
		''' 
		''' - a parametrized query used to retrieve one item<br />
		''' 
		''' - a set a values to use to prepare the statement<br /><br />
		''' 
		''' The parametrized query is passed at construction time and it should fit the main record's reader query. For
		''' instance, one could have to following reader query : "SELECT * FROM Items", and a corresponding metadata query
		''' could be "SELECT * FROM Items WHERE id = ?". For each record, the columns indicated in <seealso cref="metadataIndices"/>
		''' will be stored. For instance, one could set metadataIndices = {0} so the value of the first column of each record
		''' is stored in the metadata.
		''' </summary>
		''' <param name="recordMetaData"> Metadata for the record that we want to load from </param>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public org.datavec.api.records.Record loadFromMetaData(org.datavec.api.records.metadata.RecordMetaData recordMetaData) throws java.io.IOException
		Public Overrides Function loadFromMetaData(ByVal recordMetaData As RecordMetaData) As Record
			Return loadFromMetaData(Collections.singletonList(recordMetaData)).get(0)
		End Function

		''' <seealso cref= #loadFromMetaData(RecordMetaData) </seealso>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public java.util.List<org.datavec.api.records.Record> loadFromMetaData(java.util.List<org.datavec.api.records.metadata.RecordMetaData> recordMetaDatas) throws java.io.IOException
		Public Overrides Function loadFromMetaData(ByVal recordMetaDatas As IList(Of RecordMetaData)) As IList(Of Record)
			Dim ret As IList(Of Record) = New List(Of Record)()

			For Each rmd As RecordMetaData In recordMetaDatas
				If Not (TypeOf rmd Is RecordMetaDataJdbc) Then
					Throw New System.ArgumentException("Invalid metadata; expected RecordMetaDataJdbc instance; got: " & rmd)
				End If
				Dim runner As New QueryRunner()
				Dim request As String = DirectCast(rmd, RecordMetaDataJdbc).getRequest()

				Try
					Dim item() As Object = runner.query(Me.conn, request, New ArrayHandler(), DirectCast(rmd, RecordMetaDataJdbc).getParams().toArray())
					ret.Add(New org.datavec.api.records.impl.Record(toWritable(item), rmd))
				Catch e As SQLException
					Throw New System.ArgumentException("Could not execute statement """ & request & """", e)
				End Try
			Next rmd
			Return ret
		End Function

		''' <summary>
		''' Expected to be called by the user. JDBC connections will not be closed automatically.
		''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void close() throws java.io.IOException
		Public Overridable Sub Dispose()
			closeJdbc()
		End Sub

		Private Sub closeJdbc()
			DbUtils.closeQuietly(statement)
			DbUtils.closeQuietly(conn)
		End Sub

		Public Overrides Property Conf As Configuration
			Set(ByVal conf As Configuration)
				Me.configuration = conf
			End Set
			Get
				Return Me.configuration
			End Get
		End Property

	End Class

End Namespace