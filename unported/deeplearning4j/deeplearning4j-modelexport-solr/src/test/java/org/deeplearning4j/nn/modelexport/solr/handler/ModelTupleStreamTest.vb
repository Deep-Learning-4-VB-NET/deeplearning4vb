Imports System
Imports System.Collections.Generic
Imports System.Text
Imports ExpressionType = org.apache.solr.client.solrj.io.stream.expr.Explanation.ExpressionType
Imports Explanation = org.apache.solr.client.solrj.io.stream.expr.Explanation
Imports StreamExplanation = org.apache.solr.client.solrj.io.stream.expr.StreamExplanation
Imports StreamExpressionParameter = org.apache.solr.client.solrj.io.stream.expr.StreamExpressionParameter
Imports StreamExpressionParser = org.apache.solr.client.solrj.io.stream.expr.StreamExpressionParser
Imports StreamExpression = org.apache.solr.client.solrj.io.stream.expr.StreamExpression
Imports StreamFactory = org.apache.solr.client.solrj.io.stream.expr.StreamFactory
Imports StreamContext = org.apache.solr.client.solrj.io.stream.StreamContext
Imports TupleStream = org.apache.solr.client.solrj.io.stream.TupleStream
Imports SolrClientCache = org.apache.solr.client.solrj.io.SolrClientCache
Imports Tuple = org.apache.solr.client.solrj.io.Tuple
Imports SolrResourceLoader = org.apache.solr.core.SolrResourceLoader
Imports SolrDefaultStreamFactory = org.apache.solr.handler.SolrDefaultStreamFactory
Imports Model = org.deeplearning4j.nn.api.Model
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports ModelGuesser = org.deeplearning4j.core.util.ModelGuesser
Imports ModelSerializer = org.deeplearning4j.util.ModelSerializer
Imports NetworkUtils = org.deeplearning4j.util.NetworkUtils
import static org.junit.jupiter.api.Assertions.assertFalse
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertTrue
import static org.junit.jupiter.api.Assertions.assertNotNull
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports DisplayName = org.junit.jupiter.api.DisplayName
Imports ExtendWith = org.junit.jupiter.api.extension.ExtendWith

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

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Model Tuple Stream Test") @Tag(TagNames.SOLR) @Tag(TagNames.DIST_SYSTEMS) class ModelTupleStreamTest
	Friend Class ModelTupleStreamTest

		Shared Sub New()
	'        
	'    This is a hack around the backend-dependent nature of secure random implementations
	'    though we can set the secure random algorithm in our pom.xml files (via maven surefire and test.solr.allowed.securerandom)
	'    there isn't a mechanism that is completely platform independent.
	'    By setting it there (for example, to NativePRNG) that makes it pass on some platforms like Linux but fails on some JVMs on Windows
	'    For testing purposes, we don't need strict guarantees around RNG, hence we don't want to enforce the RNG algorithm
	'     
			Dim algorithm As String = (New SecureRandom()).getAlgorithm()
			System.setProperty("test.solr.allowed.securerandom", algorithm)
		End Sub

		Protected Friend Overridable Function floatsList(ByVal numFloats As Integer) As IList(Of Single())
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.List<float[]> floatsList = new java.util.ArrayList<float[]>();
'JAVA TO VB CONVERTER NOTE: The local variable floatsList was renamed since Visual Basic will not allow local variables with the same name as their enclosing function or property:
			Dim floatsList_Conflict As IList(Of Single()) = New List(Of Single())()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final float[] floats0 = new float[numFloats];
			Dim floats0(numFloats - 1) As Single
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final float[] floats1 = new float[numFloats];
			Dim floats1(numFloats - 1) As Single
			For ii As Integer = 0 To numFloats - 1
				floats0(ii) = 0f
				floats1(ii) = 1f
			Next ii
			floatsList_Conflict.Add(floats0)
			floatsList_Conflict.Add(floats1)
			Return floatsList_Conflict
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Test") @Disabled("Permissions issues on CI") void test() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub test()
			Dim testsCount As Integer = 0
			For numInputs As Integer = 1 To 5
				For numOutputs As Integer = 1 To 5
					For Each model As Model In New Model() { buildMultiLayerNetworkModel(numInputs, numOutputs), buildComputationGraphModel(numInputs, numOutputs) }
						doTest(model, numInputs, numOutputs)
						testsCount += 1
					Next model
				Next numOutputs
			Next numInputs
			assertEquals(50, testsCount)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void doTest(org.deeplearning4j.nn.api.Model originalModel, int numInputs, int numOutputs) throws Exception
		Private Sub doTest(ByVal originalModel As Model, ByVal numInputs As Integer, ByVal numOutputs As Integer)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.nio.file.Path tempDirPath = java.nio.file.Files.createTempDirectory(null);
			Dim tempDirPath As Path = Files.createTempDirectory(Nothing)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.io.File tempDirFile = tempDirPath.toFile();
			Dim tempDirFile As File = tempDirPath.toFile()
			tempDirFile.deleteOnExit()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.apache.solr.core.SolrResourceLoader solrResourceLoader = new org.apache.solr.core.SolrResourceLoader(tempDirPath);
			Dim solrResourceLoader As New SolrResourceLoader(tempDirPath)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.io.File tempFile = java.io.File.createTempFile("prefix", "suffix", tempDirFile);
			Dim tempFile As File = File.createTempFile("prefix", "suffix", tempDirFile)
			tempFile.deleteOnExit()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String serializedModelFileName = tempFile.getPath();
			Dim serializedModelFileName As String = tempFile.getPath()
			ModelSerializer.writeModel(originalModel, serializedModelFileName, False)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.nn.api.Model restoredModel = org.deeplearning4j.core.util.ModelGuesser.loadModelGuess(serializedModelFileName);
			Dim restoredModel As Model = ModelGuesser.loadModelGuess(serializedModelFileName)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.apache.solr.client.solrj.io.stream.StreamContext streamContext = new org.apache.solr.client.solrj.io.stream.StreamContext();
			Dim streamContext As New StreamContext()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.apache.solr.client.solrj.io.SolrClientCache solrClientCache = new org.apache.solr.client.solrj.io.SolrClientCache();
			Dim solrClientCache As New SolrClientCache()
			streamContext.setSolrClientCache(solrClientCache)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String[] inputKeys = new String[numInputs];
			Dim inputKeys(numInputs - 1) As String
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String inputKeysList = fillArray(inputKeys, "input", ",");
			Dim inputKeysList As String = fillArray(inputKeys, "input", ",")
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String[] outputKeys = new String[numOutputs];
			Dim outputKeys(numOutputs - 1) As String
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String outputKeysList = fillArray(outputKeys, "output", ",");
			Dim outputKeysList As String = fillArray(outputKeys, "output", ",")
			For Each floats As Single() In floatsList(numInputs)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String inputValuesList;
				Dim inputValuesList As String
				If True Then
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
					Dim sb As New StringBuilder()
					For ii As Integer = 0 To inputKeys.Length - 1
						If 0 < ii Then
							sb.Append(","c)
						End If
						sb.Append(inputKeys(ii)).Append("="c).Append(floats(ii))
					Next ii
					inputValuesList = sb.ToString()
				End If
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.apache.solr.client.solrj.io.stream.expr.StreamFactory streamFactory = new org.apache.solr.handler.SolrDefaultStreamFactory().withSolrResourceLoader(solrResourceLoader).withFunctionName("model", ModelTupleStream.class);
				Dim streamFactory As StreamFactory = (New SolrDefaultStreamFactory()).withSolrResourceLoader(solrResourceLoader).withFunctionName("model", GetType(ModelTupleStream))
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.apache.solr.client.solrj.io.stream.expr.StreamExpression streamExpression = org.apache.solr.client.solrj.io.stream.expr.StreamExpressionParser.parse("model(" + "tuple(" + inputValuesList + ")" + ",serializedModelFileName=""" + serializedModelFileName + """" + ",inputKeys=""" + inputKeysList + """" + ",outputKeys=""" + outputKeysList + """" + ")");
				Dim streamExpression As StreamExpression = StreamExpressionParser.parse("model(" & "tuple(" & inputValuesList & ")" & ",serializedModelFileName=""" & serializedModelFileName & """" & ",inputKeys=""" & inputKeysList & """" & ",outputKeys=""" & outputKeysList & """" & ")")
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.apache.solr.client.solrj.io.stream.TupleStream tupleStream = streamFactory.constructStream(streamExpression);
				Dim tupleStream As TupleStream = streamFactory.constructStream(streamExpression)
				tupleStream.setStreamContext(streamContext)
				assertTrue(TypeOf tupleStream Is ModelTupleStream)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final ModelTupleStream modelTupleStream = (ModelTupleStream) tupleStream;
				Dim modelTupleStream As ModelTupleStream = DirectCast(tupleStream, ModelTupleStream)
				modelTupleStream.open()
				If True Then
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.apache.solr.client.solrj.io.Tuple tuple1 = modelTupleStream.read();
					Dim tuple1 As Tuple = modelTupleStream.read()
					assertNotNull(tuple1)
					assertFalse(tuple1.EOF)
					For ii As Integer = 0 To outputKeys.Length - 1
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray inputs = org.nd4j.linalg.factory.Nd4j.create(new float[][] { floats });
						Dim inputs As INDArray = Nd4j.create(New Single()() { floats })
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final double originalScore = org.deeplearning4j.util.NetworkUtils.output((org.deeplearning4j.nn.api.Model) originalModel, inputs).getDouble(ii);
						Dim originalScore As Double = NetworkUtils.output(DirectCast(originalModel, Model), inputs).getDouble(ii)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final double restoredScore = org.deeplearning4j.util.NetworkUtils.output((org.deeplearning4j.nn.api.Model) restoredModel, inputs).getDouble(ii);
						Dim restoredScore As Double = NetworkUtils.output(DirectCast(restoredModel, Model), inputs).getDouble(ii)
						assertEquals(originalScore, restoredScore, 1e-5,originalModel.GetType().Name & " (originalScore-restoredScore)=" & (originalScore - restoredScore))
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final System.Nullable<Double> outputValue = tuple1.getDouble(outputKeys[ii]);
						Dim outputValue As Double? = tuple1.getDouble(outputKeys(ii))
						assertNotNull(outputValue)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final double tupleScore = outputValue.doubleValue();
						Dim tupleScore As Double = outputValue.Value
						assertEquals(originalScore, tupleScore, 1e-5,originalModel.GetType().Name & " (originalScore-tupleScore[" & ii & "])=" & (originalScore - tupleScore))
					Next ii
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.apache.solr.client.solrj.io.Tuple tuple2 = modelTupleStream.read();
					Dim tuple2 As Tuple = modelTupleStream.read()
					assertNotNull(tuple2)
					assertTrue(tuple2.EOF)
				End If
				modelTupleStream.close()
				doToExpressionTest(streamExpression, modelTupleStream.toExpression(streamFactory), inputKeys.Length)
				doToExplanationTest(modelTupleStream.toExplanation(streamFactory))
			Next floats
		End Sub

		Private Shared Sub doToExpressionTest(ByVal streamExpression As StreamExpression, ByVal streamExpressionParameter As StreamExpressionParameter, ByVal inputKeysLength As Integer)
			assertTrue(TypeOf streamExpressionParameter Is StreamExpression)
			' tuple(input1=1,input2=2) and tuple(input2=2,input1=1) are equivalent
			' but StreamExpression equals does not consider them equal.
			If inputKeysLength = 1 Then
				assertEquals(streamExpression, CType(streamExpressionParameter, StreamExpression))
			End If
		End Sub

		Private Shared Sub doToExplanationTest(ByVal explanation As Explanation)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.Map<String, Object> explanationMap = new java.util.TreeMap<String, Object>();
			Dim explanationMap As IDictionary(Of String, Object) = New SortedDictionary(Of String, Object)()
			explanation.toMap(explanationMap)
			assertTrue(TypeOf explanation Is StreamExplanation)
			assertNotNull(explanationMap.Remove("children"))
			assertNotNull(explanationMap.Remove("expression"))
			assertNotNull(explanationMap.Remove("expressionNodeId"))
			assertEquals(ExpressionType.STREAM_DECORATOR, explanationMap.Remove("expressionType"))
			assertEquals(explanationMap.Remove("functionName"), "model")
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			assertEquals(GetType(ModelTupleStream).FullName, explanationMap.Remove("implementingClass"))
			assertTrue(explanationMap.Count = 0,explanationMap.ToString())
		End Sub

		''' <summary>
		''' Fills an existing array using prefix and delimiter, e.g.
		''' input: arr = [ "", "", "" ] prefix="value" delimiter=","
		''' output: arr = [ "value1", "value2", "value3" ]
		''' return: "value1,value2,value3"
		''' </summary>
'JAVA TO VB CONVERTER WARNING: 'final' parameters are not available in VB:
'ORIGINAL LINE: private static String fillArray(String[] arr, final String prefix, final String delimiter)
		Private Shared Function fillArray(ByVal arr() As String, ByVal prefix As String, ByVal delimiter As String) As String
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final StringBuilder sb = new StringBuilder();
			Dim sb As New StringBuilder()
			For ii As Integer = 0 To arr.Length - 1
				arr(ii) = prefix & Convert.ToString(ii + 1)
				If 0 < ii Then
					sb.Append(delimiter)
				End If
				sb.Append(arr(ii))
			Next ii
			Return sb.ToString()
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: protected org.deeplearning4j.nn.api.Model buildMultiLayerNetworkModel(int numInputs, int numOutputs) throws Exception
		Protected Friend Overridable Function buildMultiLayerNetworkModel(ByVal numInputs As Integer, ByVal numOutputs As Integer) As Model
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.nn.conf.MultiLayerConfiguration conf = new org.deeplearning4j.nn.conf.NeuralNetConfiguration.Builder().list(new org.deeplearning4j.nn.conf.layers.OutputLayer.Builder().nIn(numInputs).nOut(numOutputs).activation(org.nd4j.linalg.activations.Activation.IDENTITY).lossFunction(org.nd4j.linalg.lossfunctions.LossFunctions.LossFunction.MSE).build()).build();
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list((New OutputLayer.Builder()).nIn(numInputs).nOut(numOutputs).activation(Activation.IDENTITY).lossFunction(LossFunctions.LossFunction.MSE).build()).build()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.nn.multilayer.MultiLayerNetwork model = new org.deeplearning4j.nn.multilayer.MultiLayerNetwork(conf);
			Dim model As New MultiLayerNetwork(conf)
			model.init()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final float[] floats = new float[(numInputs + 1) * numOutputs];
			Dim floats(((numInputs + 1) * numOutputs) - 1) As Single
			Const base0 As Single = 0.01f
			Dim base As Single = base0
			For ii As Integer = 0 To floats.Length - 1
				base *= 2
				If base > 1 / base0 Then
					base = base0
				End If
				floats(ii) = base
			Next ii
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray params = org.nd4j.linalg.factory.Nd4j.create(floats);
			Dim params As INDArray = Nd4j.create(floats)
			model.Params = params
			Return model
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: protected org.deeplearning4j.nn.api.Model buildComputationGraphModel(int numInputs, int numOutputs) throws Exception
		Protected Friend Overridable Function buildComputationGraphModel(ByVal numInputs As Integer, ByVal numOutputs As Integer) As Model
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.nn.conf.ComputationGraphConfiguration conf = new org.deeplearning4j.nn.conf.NeuralNetConfiguration.Builder().graphBuilder().addInputs("inputLayer").addLayer("outputLayer", new org.deeplearning4j.nn.conf.layers.OutputLayer.Builder().nIn(numInputs).nOut(numOutputs).activation(org.nd4j.linalg.activations.Activation.IDENTITY).lossFunction(org.nd4j.linalg.lossfunctions.LossFunctions.LossFunction.MSE).build(), "inputLayer").setOutputs("outputLayer").build();
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("inputLayer").addLayer("outputLayer", (New OutputLayer.Builder()).nIn(numInputs).nOut(numOutputs).activation(Activation.IDENTITY).lossFunction(LossFunctions.LossFunction.MSE).build(), "inputLayer").setOutputs("outputLayer").build()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.nn.graph.ComputationGraph model = new org.deeplearning4j.nn.graph.ComputationGraph(conf);
			Dim model As New ComputationGraph(conf)
			model.init()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final float[] floats = new float[(numInputs + 1) * numOutputs];
			Dim floats(((numInputs + 1) * numOutputs) - 1) As Single
			Const base0 As Single = 0.01f
			Dim base As Single = base0
			For ii As Integer = 0 To floats.Length - 1
				base *= 2
				If base > 1 / base0 Then
					base = base0
				End If
				floats(ii) = base
			Next ii
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray params = org.nd4j.linalg.factory.Nd4j.create(floats);
			Dim params As INDArray = Nd4j.create(floats)
			model.Params = params
			Return model
		End Function
	End Class

End Namespace