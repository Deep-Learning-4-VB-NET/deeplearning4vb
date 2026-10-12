Imports System
Imports System.Collections.Generic
Imports Getter = lombok.Getter
Imports SolrClientCache = org.apache.solr.client.solrj.io.SolrClientCache
Imports Tuple = org.apache.solr.client.solrj.io.Tuple
Imports CloudSolrStream = org.apache.solr.client.solrj.io.stream.CloudSolrStream
Imports TupStream = org.apache.solr.client.solrj.io.stream.TupStream
Imports StreamContext = org.apache.solr.client.solrj.io.stream.StreamContext
Imports TupleStream = org.apache.solr.client.solrj.io.stream.TupleStream
Imports DefaultStreamFactory = org.apache.solr.client.solrj.io.stream.expr.DefaultStreamFactory
Imports StreamFactory = org.apache.solr.client.solrj.io.stream.expr.StreamFactory
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports DataSetPreProcessor = org.nd4j.linalg.dataset.api.DataSetPreProcessor
Imports DataSetIterator = org.nd4j.linalg.dataset.api.iterator.DataSetIterator
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory

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

Namespace org.deeplearning4j.nn.dataimport.solr.client.solrj.io.stream

	<Serializable>
	Public Class TupleStreamDataSetIterator
		Implements Closeable, DataSetIterator

		Private Shared ReadOnly log As Logger = LoggerFactory.getLogger(MethodHandles.lookup().lookupClass())

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected org.nd4j.linalg.dataset.api.DataSetPreProcessor preProcessor;
'JAVA TO VB CONVERTER NOTE: The field preProcessor was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Protected Friend preProcessor_Conflict As DataSetPreProcessor

'JAVA TO VB CONVERTER NOTE: The field batch was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly batch_Conflict As Integer
		Private ReadOnly idKey As String
		Private ReadOnly featureKeys() As String
		Private ReadOnly labelKeys() As String

		Private streamContext As StreamContext
		Private tupleStream As TupleStream
		Private tuple As Tuple

		Private Class CloseableStreamContext
			Inherits StreamContext
			Implements System.IDisposable

			Friend solrClientCache As SolrClientCache
			Public Sub New()
				solrClientCache = New SolrClientCache()
				setSolrClientCache(solrClientCache)
			End Sub
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void close() throws java.io.IOException
			Public Overridable Sub Dispose() Implements System.IDisposable.Dispose
				If solrClientCache IsNot Nothing Then
					solrClientCache.close()
				End If
			End Sub
		End Class

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public TupleStreamDataSetIterator(int batch, String idKey, String[] featureKeys, String[] labelKeys, String expression, String defaultZkHost) throws java.io.IOException
		Public Sub New(ByVal batch As Integer, ByVal idKey As String, ByVal featureKeys() As String, ByVal labelKeys() As String, ByVal expression As String, ByVal defaultZkHost As String)

			Me.New(batch, idKey, featureKeys, labelKeys, (New DefaultStreamFactory()).withDefaultZkHost(defaultZkHost), expression)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public TupleStreamDataSetIterator(int batch, String idKey, String[] featureKeys, String[] labelKeys, org.apache.solr.client.solrj.io.stream.expr.StreamFactory streamFactory, String expression) throws java.io.IOException
		Public Sub New(ByVal batch As Integer, ByVal idKey As String, ByVal featureKeys() As String, ByVal labelKeys() As String, ByVal streamFactory As StreamFactory, ByVal expression As String)

			Me.New(batch, idKey, featureKeys, labelKeys, streamFactory, expression, New CloseableStreamContext())
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public TupleStreamDataSetIterator(int batch, String idKey, String[] featureKeys, String[] labelKeys, org.apache.solr.client.solrj.io.stream.expr.StreamFactory streamFactory, String expression, org.apache.solr.client.solrj.io.stream.StreamContext streamContext) throws java.io.IOException
		Public Sub New(ByVal batch As Integer, ByVal idKey As String, ByVal featureKeys() As String, ByVal labelKeys() As String, ByVal streamFactory As StreamFactory, ByVal expression As String, ByVal streamContext As StreamContext)

			Me.batch_Conflict = batch
			Me.idKey = idKey
			Me.featureKeys = featureKeys
			Me.labelKeys = labelKeys

			Me.streamContext = streamContext
			Me.tupleStream = streamFactory.constructStream(expression)
			Me.tupleStream.setStreamContext(streamContext)

			Me.tupleStream.open()
			Me.tuple = Me.tupleStream.read()
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void close() throws java.io.IOException
		Public Overridable Sub Dispose() Implements System.IDisposable.Dispose
			Me.tuple = Nothing
			If Me.tupleStream IsNot Nothing Then
				Me.tupleStream.close()
				Me.tupleStream = Nothing
			End If
			If Me.streamContext IsNot Nothing AndAlso TypeOf Me.streamContext Is CloseableStreamContext Then
				DirectCast(Me.streamContext, CloseableStreamContext).Dispose()
				Me.streamContext = Nothing
			End If
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private java.util.List<org.nd4j.linalg.dataset.DataSet> getRawDataSets(int numWanted) throws java.io.IOException
		Private Function getRawDataSets(ByVal numWanted As Integer) As IList(Of DataSet)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.List<org.nd4j.linalg.dataset.DataSet> rawDataSets = new java.util.ArrayList<org.nd4j.linalg.dataset.DataSet>();
			Dim rawDataSets As IList(Of DataSet) = New List(Of DataSet)()

'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
			Do While hasNext() AndAlso 0 < numWanted

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray features = getValues(this.tuple, this.featureKeys, this.idKey);
				Dim features As INDArray = getValues(Me.tuple, Me.featureKeys, Me.idKey)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray labels = getValues(this.tuple, this.labelKeys, this.idKey);
				Dim labels As INDArray = getValues(Me.tuple, Me.labelKeys, Me.idKey)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.dataset.DataSet rawDataSet = new org.nd4j.linalg.dataset.DataSet(features, labels);
				Dim rawDataSet As New DataSet(features, labels)
				rawDataSets.Add(rawDataSet)

				numWanted -= 1
				Me.tuple = Me.tupleStream.read()
			Loop

			Return rawDataSets
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private org.nd4j.linalg.dataset.DataSet convertDataSetsToDataSet(java.util.List<org.nd4j.linalg.dataset.DataSet> rawDataSets) throws java.io.IOException
		Private Function convertDataSetsToDataSet(ByVal rawDataSets As IList(Of DataSet)) As DataSet

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int numFound = rawDataSets.size();
			Dim numFound As Integer = rawDataSets.Count

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray inputs = org.nd4j.linalg.factory.Nd4j.create(numFound, inputColumns());
			Dim inputs As INDArray = Nd4j.create(numFound, inputColumns())
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray labels = org.nd4j.linalg.factory.Nd4j.create(numFound, totalOutcomes());
			Dim labels As INDArray = Nd4j.create(numFound, totalOutcomes())
			For ii As Integer = 0 To numFound - 1
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.dataset.DataSet dataSet = rawDataSets.get(ii);
				Dim dataSet As DataSet = rawDataSets(ii)
				If preProcessor_Conflict IsNot Nothing Then
					preProcessor_Conflict.preProcess(dataSet)
				End If
				inputs.putRow(ii, dataSet.Features)
				labels.putRow(ii, dataSet.Labels)
			Next ii

			Return New DataSet(inputs, labels)
		End Function

		Private Shared Function getValues(ByVal tuple As Tuple, ByVal keys() As String, ByVal idKey As String) As INDArray
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final double[] values = new double[keys.length];
		  Dim values(keys.Length - 1) As Double
		  For ii As Integer = 0 To keys.Length - 1
			values(ii) = getValue(tuple, keys(ii), idKey)
		  Next ii
		  Return Nd4j.create(values)
		End Function

		Private Shared Function getValue(ByVal tuple As Tuple, ByVal key As String, ByVal idKey As String) As Double
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final System.Nullable<Double> value = tuple.getDouble(key);
			Dim value As Double? = tuple.getDouble(key)
			If value Is Nothing Then
			  ' log potentially useful debugging info here ...
			  If idKey Is Nothing Then
				log.info("tuple[{}]={}", key, value)
			  Else
				log.info("tuple[{}]={} tuple[{}]={}", key, value, idKey, tuple.get(idKey))
			  End If
			  ' ... before proceeding to hit the NullPointerException below
			End If
			Return value.Value
		End Function

		Public Overrides Function hasNext() As Boolean
			Return Me.tuple IsNot Nothing AndAlso Not Me.tuple.EOF
		End Function

		Public Overrides Function [next]() As DataSet
			Return [next](batch_Conflict)
		End Function

		Public Overridable Function [next](ByVal num As Integer) As DataSet Implements DataSetIterator.next
			Try
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.List<org.nd4j.linalg.dataset.DataSet> rawDataSets = getRawDataSets(num);
				Dim rawDataSets As IList(Of DataSet) = getRawDataSets(num)
				Return convertDataSetsToDataSet(rawDataSets)
			Catch ioe As IOException
				Return Nothing
			End Try
		End Function

		Public Overrides Sub remove()
			Throw New System.NotSupportedException()
		End Sub

		Public Overridable Function inputColumns() As Integer Implements DataSetIterator.inputColumns
			Return Me.featureKeys.Length
		End Function

		Public Overridable Function totalOutcomes() As Integer Implements DataSetIterator.totalOutcomes
			Return Me.labelKeys.Length
		End Function

		Public Overridable Function resetSupported() As Boolean Implements DataSetIterator.resetSupported
			Return False
		End Function

		Public Overridable Function asyncSupported() As Boolean Implements DataSetIterator.asyncSupported
			Return False
		End Function

		Public Overridable Sub reset() Implements DataSetIterator.reset
			Throw New System.NotSupportedException()
		End Sub

		Public Overridable Function batch() As Integer Implements DataSetIterator.batch
			Return Me.batch_Conflict
		End Function

		Public Overridable WriteOnly Property PreProcessor Implements DataSetIterator.setPreProcessor As DataSetPreProcessor
			Set(ByVal preProcessor As DataSetPreProcessor)
				Me.preProcessor_Conflict = preProcessor
			End Set
		End Property

		Public Overridable ReadOnly Property Labels As IList(Of String) Implements DataSetIterator.getLabels
			Get
				Throw New System.NotSupportedException()
			End Get
		End Property

	End Class

End Namespace