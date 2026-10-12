Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports Explanation = org.apache.lucene.search.Explanation
Imports IndexSearcher = org.apache.lucene.search.IndexSearcher
Imports Query = org.apache.lucene.search.Query
Imports SolrResourceLoader = org.apache.solr.core.SolrResourceLoader
Imports Feature = org.apache.solr.ltr.feature.Feature
Imports FeatureException = org.apache.solr.ltr.feature.FeatureException
Imports ModelException = org.apache.solr.ltr.model.ModelException
Imports IdentityNormalizer = org.apache.solr.ltr.norm.IdentityNormalizer
Imports Normalizer = org.apache.solr.ltr.norm.Normalizer
Imports SolrQueryRequest = org.apache.solr.request.SolrQueryRequest
Imports Model = org.deeplearning4j.nn.api.Model
Imports ComputationGraphConfiguration = org.deeplearning4j.nn.conf.ComputationGraphConfiguration
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports ComputationGraph = org.deeplearning4j.nn.graph.ComputationGraph
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports ModelGuesser = org.deeplearning4j.core.util.ModelGuesser
Imports ModelSerializer = org.deeplearning4j.util.ModelSerializer
Imports Tag = org.junit.jupiter.api.Tag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertTrue
import static org.junit.jupiter.api.Assertions.fail
Imports Test = org.junit.jupiter.api.Test
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
Namespace org.deeplearning4j.nn.modelexport.solr.ltr.model

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Scoring Model Test") @Tag(TagNames.SOLR) @Tag(TagNames.DIST_SYSTEMS) class ScoringModelTest
	Friend Class ScoringModelTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Dummy Feature") protected static class DummyFeature extends org.apache.solr.ltr.feature.Feature
		Protected Friend Class DummyFeature
			Inherits Feature

			Public Sub New(ByVal name As String)
				MyBase.New(name, Collections.EMPTY_MAP)
			End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override protected void validate() throws org.apache.solr.ltr.feature.FeatureException
			Protected Friend Overrides Sub validate()
			End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public FeatureWeight createWeight(org.apache.lucene.search.IndexSearcher searcher, boolean needsScores, org.apache.solr.request.SolrQueryRequest request, org.apache.lucene.search.Query originalQuery, java.util.Map<String, String[]> efi) throws java.io.IOException
			Public Overrides Function createWeight(ByVal searcher As IndexSearcher, ByVal needsScores As Boolean, ByVal request As SolrQueryRequest, ByVal originalQuery As Query, ByVal efi As IDictionary(Of String, String())) As FeatureWeight
				Return Nothing
			End Function

			Public Overrides Function paramsToMap() As LinkedHashMap(Of String, Object)
				Return Nothing
			End Function
		End Class

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: protected java.util.List<org.apache.solr.ltr.feature.Feature> featuresList(int numFeatures) throws Exception
		Protected Friend Overridable Function featuresList(ByVal numFeatures As Integer) As IList(Of Feature)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.ArrayList<org.apache.solr.ltr.feature.Feature> features = new java.util.ArrayList<org.apache.solr.ltr.feature.Feature>();
			Dim features As New List(Of Feature)()
			For ii As Integer = 1 To numFeatures
				features.Add(New DummyFeature("dummy" & ii))
			Next ii
			Return features
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: protected java.util.List<org.apache.solr.ltr.norm.Normalizer> normalizersList(int numNormalizers) throws Exception
		Protected Friend Overridable Function normalizersList(ByVal numNormalizers As Integer) As IList(Of Normalizer)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.ArrayList<org.apache.solr.ltr.norm.Normalizer> normalizers = new java.util.ArrayList<org.apache.solr.ltr.norm.Normalizer>();
			Dim normalizers As New List(Of Normalizer)()
			For ii As Integer = 1 To numNormalizers
				normalizers.Add(IdentityNormalizer.INSTANCE)
			Next ii
			Return normalizers
		End Function

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
'ORIGINAL LINE: @Test @DisplayName("Test") void test() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub test()
			For numFeatures As Integer = 3 To 5
				For Each model As Model In New Model() { buildMultiLayerNetworkModel(numFeatures), buildComputationGraphModel(numFeatures) }
					doTest(model, numFeatures)
				Next model
			Next numFeatures
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void doTest(org.deeplearning4j.nn.api.Model originalModel, int numFeatures) throws Exception
		Private Sub doTest(ByVal originalModel As Model, ByVal numFeatures As Integer)
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
'ORIGINAL LINE: final ScoringModel ltrModel = new ScoringModel("myModel", featuresList(numFeatures), normalizersList(numFeatures), null, null, null);
			Dim ltrModel As New ScoringModel("myModel", featuresList(numFeatures), normalizersList(numFeatures), Nothing, Nothing, Nothing)
			ltrModel.SerializedModelFileName = serializedModelFileName
			ltrModel.init(solrResourceLoader)
			For Each floats As Single() In floatsList(numFeatures)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final float originalScore = ScoringModel.outputScore((org.deeplearning4j.nn.api.Model) originalModel, floats);
				Dim originalScore As Single = ScoringModel.outputScore(DirectCast(originalModel, Model), floats)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final float restoredScore = ScoringModel.outputScore((org.deeplearning4j.nn.api.Model) restoredModel, floats);
				Dim restoredScore As Single = ScoringModel.outputScore(DirectCast(restoredModel, Model), floats)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final float ltrScore = ltrModel.score(floats);
				Dim ltrScore As Single = ltrModel.score(floats)
				assertEquals(originalScore, restoredScore, 0f)
				assertEquals(originalScore, ltrScore, 0f)
				If 3 = numFeatures Then
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.List<org.apache.lucene.search.Explanation> explanations = new java.util.ArrayList<org.apache.lucene.search.Explanation>();
					Dim explanations As IList(Of Explanation) = New List(Of Explanation)()
					explanations.Add(Explanation.match(floats(0), ""))
					explanations.Add(Explanation.match(floats(1), ""))
					explanations.Add(Explanation.match(floats(2), ""))
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.apache.lucene.search.Explanation explanation = ltrModel.explain(null, 0, ltrScore, explanations);
					Dim explanation As Explanation = ltrModel.explain(Nothing, 0, ltrScore, explanations)
					assertEquals(ltrScore & " = (name=myModel" & ",class=" & ltrModel.GetType().Name & ",featureValues=" & "[dummy1=" & Convert.ToString(floats(0)) & ",dummy2=" & Convert.ToString(floats(1)) & ",dummy3=" & Convert.ToString(floats(2)) & "])" & vbLf, explanation.ToString())
				End If
			Next floats
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final ScoringModel invalidLtrModel = new ScoringModel("invalidModel", featuresList(numFeatures + 1), normalizersList(numFeatures + 1), null, null, null);
			Dim invalidLtrModel As New ScoringModel("invalidModel", featuresList(numFeatures + 1), normalizersList(numFeatures + 1), Nothing, Nothing, Nothing)
			invalidLtrModel.SerializedModelFileName = serializedModelFileName
			Try
				invalidLtrModel.init(solrResourceLoader)
				fail("expected to exception from invalid model init")
			Catch [me] As ModelException
				assertTrue([me].Message.StartsWith("score(...) test failed for model "))
			End Try
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: protected org.deeplearning4j.nn.api.Model buildMultiLayerNetworkModel(int numFeatures) throws Exception
		Protected Friend Overridable Function buildMultiLayerNetworkModel(ByVal numFeatures As Integer) As Model
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.nn.conf.MultiLayerConfiguration conf = new org.deeplearning4j.nn.conf.NeuralNetConfiguration.Builder().list(new org.deeplearning4j.nn.conf.layers.OutputLayer.Builder().nIn(numFeatures).nOut(1).lossFunction(org.nd4j.linalg.lossfunctions.LossFunctions.LossFunction.MSE).activation(org.nd4j.linalg.activations.Activation.IDENTITY).build()).build();
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list((New OutputLayer.Builder()).nIn(numFeatures).nOut(1).lossFunction(LossFunctions.LossFunction.MSE).activation(Activation.IDENTITY).build()).build()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.nn.multilayer.MultiLayerNetwork model = new org.deeplearning4j.nn.multilayer.MultiLayerNetwork(conf);
			Dim model As New MultiLayerNetwork(conf)
			model.init()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final float[] floats = new float[numFeatures + 1];
			Dim floats(numFeatures) As Single
			Dim base As Single = 1f
			For ii As Integer = 0 To floats.Length - 1
				base *= 2
				floats(ii) = base
			Next ii
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray params = org.nd4j.linalg.factory.Nd4j.create(floats);
			Dim params As INDArray = Nd4j.create(floats)
			model.Params = params
			Return model
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: protected org.deeplearning4j.nn.api.Model buildComputationGraphModel(int numFeatures) throws Exception
		Protected Friend Overridable Function buildComputationGraphModel(ByVal numFeatures As Integer) As Model
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.nn.conf.ComputationGraphConfiguration conf = new org.deeplearning4j.nn.conf.NeuralNetConfiguration.Builder().graphBuilder().addInputs("inputLayer").addLayer("outputLayer", new org.deeplearning4j.nn.conf.layers.OutputLayer.Builder().nIn(numFeatures).nOut(1).lossFunction(org.nd4j.linalg.lossfunctions.LossFunctions.LossFunction.MSE).activation(org.nd4j.linalg.activations.Activation.IDENTITY).build(), "inputLayer").setOutputs("outputLayer").build();
			Dim conf As ComputationGraphConfiguration = (New NeuralNetConfiguration.Builder()).graphBuilder().addInputs("inputLayer").addLayer("outputLayer", (New OutputLayer.Builder()).nIn(numFeatures).nOut(1).lossFunction(LossFunctions.LossFunction.MSE).activation(Activation.IDENTITY).build(), "inputLayer").setOutputs("outputLayer").build()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.nn.graph.ComputationGraph model = new org.deeplearning4j.nn.graph.ComputationGraph(conf);
			Dim model As New ComputationGraph(conf)
			model.init()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final float[] floats = new float[numFeatures + 1];
			Dim floats(numFeatures) As Single
			Dim base As Single = 1f
			For ii As Integer = 0 To floats.Length - 1
				base *= 2
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