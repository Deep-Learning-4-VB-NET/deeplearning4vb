Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Tuple = org.apache.solr.client.solrj.io.Tuple
Imports StreamComparator = org.apache.solr.client.solrj.io.comp.StreamComparator
Imports StreamContext = org.apache.solr.client.solrj.io.stream.StreamContext
Imports TupleStream = org.apache.solr.client.solrj.io.stream.TupleStream
Imports ExpressionType = org.apache.solr.client.solrj.io.stream.expr.Explanation.ExpressionType
Imports Explanation = org.apache.solr.client.solrj.io.stream.expr.Explanation
Imports Expressible = org.apache.solr.client.solrj.io.stream.expr.Expressible
Imports StreamExplanation = org.apache.solr.client.solrj.io.stream.expr.StreamExplanation
Imports StreamExpression = org.apache.solr.client.solrj.io.stream.expr.StreamExpression
Imports StreamExpressionNamedParameter = org.apache.solr.client.solrj.io.stream.expr.StreamExpressionNamedParameter
Imports StreamExpressionParameter = org.apache.solr.client.solrj.io.stream.expr.StreamExpressionParameter
Imports StreamExpressionValue = org.apache.solr.client.solrj.io.stream.expr.StreamExpressionValue
Imports StreamFactory = org.apache.solr.client.solrj.io.stream.expr.StreamFactory
Imports SolrResourceLoader = org.apache.solr.core.SolrResourceLoader
Imports SolrDefaultStreamFactory = org.apache.solr.handler.SolrDefaultStreamFactory
Imports Model = org.deeplearning4j.nn.api.Model
Imports ModelGuesser = org.deeplearning4j.core.util.ModelGuesser
Imports NetworkUtils = org.deeplearning4j.util.NetworkUtils
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.deeplearning4j.nn.modelexport.solr.handler

	Public Class ModelTupleStream
		Inherits TupleStream
		Implements Expressible

	  Private Const SERIALIZED_MODEL_FILE_NAME_PARAM As String = "serializedModelFileName"
	  Private Const INPUT_KEYS_PARAM As String = "inputKeys"
	  Private Const OUTPUT_KEYS_PARAM As String = "outputKeys"

	  Private ReadOnly tupleStream As TupleStream
	  Private ReadOnly serializedModelFileName As String
	  Private ReadOnly inputKeysParam As String
	  Private ReadOnly outputKeysParam As String
	  Private ReadOnly inputKeys() As String
	  Private ReadOnly outputKeys() As String
	  Private ReadOnly solrResourceLoader As SolrResourceLoader
	  Private ReadOnly model As Model

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public ModelTupleStream(org.apache.solr.client.solrj.io.stream.expr.StreamExpression streamExpression, org.apache.solr.client.solrj.io.stream.expr.StreamFactory streamFactory) throws java.io.IOException
	  Public Sub New(ByVal streamExpression As StreamExpression, ByVal streamFactory As StreamFactory)

'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.List<org.apache.solr.client.solrj.io.stream.expr.StreamExpression> streamExpressions = streamFactory.getExpressionOperandsRepresentingTypes(streamExpression, org.apache.solr.client.solrj.io.stream.expr.Expressible.class, org.apache.solr.client.solrj.io.stream.TupleStream.class);
		Dim streamExpressions As IList(Of StreamExpression) = streamFactory.getExpressionOperandsRepresentingTypes(streamExpression, GetType(Expressible), GetType(TupleStream))
		If streamExpressions.Count = 1 Then
		  Me.tupleStream = streamFactory.constructStream(streamExpressions(0))
		Else
		  Throw New IOException("Expected exactly one stream in expression: " & streamExpression)
		End If

		Me.serializedModelFileName = getOperandValue(streamExpression, streamFactory, SERIALIZED_MODEL_FILE_NAME_PARAM)

		Me.inputKeysParam = getOperandValue(streamExpression, streamFactory, INPUT_KEYS_PARAM)
		Me.inputKeys = inputKeysParam.Split(",", True)

		Me.outputKeysParam = getOperandValue(streamExpression, streamFactory, OUTPUT_KEYS_PARAM)
		Me.outputKeys = outputKeysParam.Split(",", True)

		If Not (TypeOf streamFactory Is SolrDefaultStreamFactory) Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  Throw New IOException(Me.GetType().FullName & " requires a " & GetType(SolrDefaultStreamFactory).FullName & " StreamFactory")
		End If
		Me.solrResourceLoader = CType(streamFactory, SolrDefaultStreamFactory).getSolrResourceLoader()

		Me.model = restoreModel(openInputStream())
	  End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static String getOperandValue(org.apache.solr.client.solrj.io.stream.expr.StreamExpression streamExpression, org.apache.solr.client.solrj.io.stream.expr.StreamFactory streamFactory, String operandName) throws java.io.IOException
	  Private Shared Function getOperandValue(ByVal streamExpression As StreamExpression, ByVal streamFactory As StreamFactory, ByVal operandName As String) As String
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.apache.solr.client.solrj.io.stream.expr.StreamExpressionNamedParameter namedParameter = streamFactory.getNamedOperand(streamExpression, operandName);
		Dim namedParameter As StreamExpressionNamedParameter = streamFactory.getNamedOperand(streamExpression, operandName)
		Dim operandValue As String = Nothing
		If namedParameter IsNot Nothing AndAlso TypeOf namedParameter.getParameter() Is StreamExpressionValue Then
		  operandValue = CType(namedParameter.getParameter(), StreamExpressionValue).getValue()
		End If
		If operandValue Is Nothing Then
		  Throw New IOException("Expected '" & operandName & "' in expression: " & streamExpression)
		Else
		  Return operandValue
		End If
	  End Function

	  Public Overridable Function toMap(ByVal map As IDictionary(Of String, Object)) As System.Collections.IDictionary
		' We (ModelTupleStream) extend TupleStream which implements MapWriter which extends MapSerializable.
		' MapSerializable says to have a toMap method.
		' org.apache.solr.common.MapWriter has a toMap method which has 'default' visibility.
		' So MapWriter.toMap here is not 'visible' but it is 'callable' it seems.
		Return MyBase.toMap(map)
	  End Function

	  Public Overridable WriteOnly Property StreamContext As StreamContext
		  Set(ByVal streamContext As StreamContext)
			tupleStream.setStreamContext(streamContext)
		  End Set
	  End Property

	  Public Overridable Function children() As IList(Of TupleStream)
		Return tupleStream.children()
	  End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void open() throws java.io.IOException
	  Public Overridable Sub open()
		tupleStream.open()
	  End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void close() throws java.io.IOException
	  Public Overridable Sub close()
		tupleStream.close()
	  End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.apache.solr.client.solrj.io.Tuple read() throws java.io.IOException
	  Public Overridable Function read() As Tuple
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.apache.solr.client.solrj.io.Tuple tuple = tupleStream.read();
		Dim tuple As Tuple = tupleStream.read()
		If tuple.EOF Then
		  Return tuple
		Else
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray inputs = getInputsFromTuple(tuple);
		  Dim inputs As INDArray = getInputsFromTuple(tuple)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray outputs = org.deeplearning4j.util.NetworkUtils.output(model, inputs);
		  Dim outputs As INDArray = NetworkUtils.output(model, inputs)
		  Return applyOutputsToTuple(tuple, outputs)
		End If
	  End Function

	  Public Overridable ReadOnly Property StreamSort As StreamComparator
		  Get
			Return tupleStream.getStreamSort()
		  End Get
	  End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.apache.solr.client.solrj.io.stream.expr.Explanation toExplanation(org.apache.solr.client.solrj.io.stream.expr.StreamFactory streamFactory) throws java.io.IOException
	  Public Overridable Function toExplanation(ByVal streamFactory As StreamFactory) As Explanation
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		Return (New StreamExplanation(getStreamNodeId().ToString())).withChildren(New Explanation(){ tupleStream.toExplanation(streamFactory) }).withExpressionType(ExpressionType.STREAM_DECORATOR).withFunctionName(streamFactory.getFunctionName(Me.GetType())).withImplementingClass(Me.GetType().FullName).withExpression(toExpression(streamFactory, False).ToString())
	  End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public org.apache.solr.client.solrj.io.stream.expr.StreamExpressionParameter toExpression(org.apache.solr.client.solrj.io.stream.expr.StreamFactory streamFactory) throws java.io.IOException
	  Public Overridable Function toExpression(ByVal streamFactory As StreamFactory) As StreamExpressionParameter
		Return toExpression(streamFactory, True)
	  End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private org.apache.solr.client.solrj.io.stream.expr.StreamExpression toExpression(org.apache.solr.client.solrj.io.stream.expr.StreamFactory streamFactory, boolean includeStreams) throws java.io.IOException
	  Private Function toExpression(ByVal streamFactory As StreamFactory, ByVal includeStreams As Boolean) As StreamExpression
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String functionName = streamFactory.getFunctionName(this.getClass());
		Dim functionName As String = streamFactory.getFunctionName(Me.GetType())
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.apache.solr.client.solrj.io.stream.expr.StreamExpression streamExpression = new org.apache.solr.client.solrj.io.stream.expr.StreamExpression(functionName);
		Dim streamExpression As New StreamExpression(functionName)

		If includeStreams Then
		  If TypeOf Me.tupleStream Is Expressible Then
			streamExpression.addParameter(CType(Me.tupleStream, Expressible).toExpression(streamFactory))
		  Else
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			Throw New IOException("This " & Me.GetType().FullName & " contains a non-Expressible TupleStream " & Me.tupleStream.GetType().FullName)
		  End If
		Else
		  streamExpression.addParameter("<stream>")
		End If

		streamExpression.addParameter(New StreamExpressionNamedParameter(SERIALIZED_MODEL_FILE_NAME_PARAM, Me.serializedModelFileName))
		streamExpression.addParameter(New StreamExpressionNamedParameter(INPUT_KEYS_PARAM, Me.inputKeysParam))
		streamExpression.addParameter(New StreamExpressionNamedParameter(OUTPUT_KEYS_PARAM, Me.outputKeysParam))

		Return streamExpression
	  End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: protected java.io.InputStream openInputStream() throws java.io.IOException
	  Protected Friend Overridable Function openInputStream() As Stream
		Return solrResourceLoader.openResource(serializedModelFileName)
	  End Function

	  ''' <summary>
	  ''' Uses the <seealso cref="ModelGuesser.loadModelGuess(InputStream)"/> method.
	  ''' </summary>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: protected org.deeplearning4j.nn.api.Model restoreModel(java.io.InputStream inputStream) throws java.io.IOException
	  Protected Friend Overridable Function restoreModel(ByVal inputStream As Stream) As Model
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.io.File instanceDir = solrResourceLoader.getInstancePath().toFile();
		Dim instanceDir As File = solrResourceLoader.getInstancePath().toFile()
		Try
		  Return ModelGuesser.loadModelGuess(inputStream, instanceDir)
		Catch e As Exception
		  Throw New IOException("Failed to restore model from given file (" & serializedModelFileName & ")", e)
		End Try
	  End Function

	  Protected Friend Overridable Function getInputsFromTuple(ByVal tuple As Tuple) As INDArray
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final double[] inputs = new double[inputKeys.length];
		Dim inputs(inputKeys.Length - 1) As Double
		For ii As Integer = 0 To inputKeys.Length - 1
		  inputs(ii) = tuple.getDouble(inputKeys(ii)).doubleValue()
		Next ii
		Return Nd4j.create(New Double()(){ inputs })
	  End Function

	  Protected Friend Overridable Function applyOutputsToTuple(ByVal tuple As Tuple, ByVal output As INDArray) As Tuple
		For ii As Integer = 0 To outputKeys.Length - 1
		  tuple.put(outputKeys(ii), output.getFloat(ii))
		Next ii
		Return tuple
	  End Function

	End Class

End Namespace