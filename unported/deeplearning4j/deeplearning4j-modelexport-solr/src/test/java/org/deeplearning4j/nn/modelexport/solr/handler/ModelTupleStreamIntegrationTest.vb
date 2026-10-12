Imports System
Imports System.Threading
Imports ThreadFilter = com.carrotsearch.randomizedtesting.ThreadFilter
Imports ThreadLeakFilters = com.carrotsearch.randomizedtesting.annotations.ThreadLeakFilters
Imports Tuple = org.apache.solr.client.solrj.io.Tuple
Imports SolrStream = org.apache.solr.client.solrj.io.stream.SolrStream
Imports StreamContext = org.apache.solr.client.solrj.io.stream.StreamContext
Imports TupleStream = org.apache.solr.client.solrj.io.stream.TupleStream
Imports CollectionAdminRequest = org.apache.solr.client.solrj.request.CollectionAdminRequest
Imports UpdateRequest = org.apache.solr.client.solrj.request.UpdateRequest
Imports SolrCloudTestCase = org.apache.solr.cloud.SolrCloudTestCase
Imports ModifiableSolrParams = org.apache.solr.common.params.ModifiableSolrParams
Imports Model = org.deeplearning4j.nn.api.Model
Imports MultiLayerConfiguration = org.deeplearning4j.nn.conf.MultiLayerConfiguration
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports ModelSerializer = org.deeplearning4j.util.ModelSerializer
Imports org.junit.jupiter.api
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports LossFunctions = org.nd4j.linalg.lossfunctions.LossFunctions
Imports BasicWorkspaceManager = org.nd4j.linalg.api.memory.provider.BasicWorkspaceManager
Imports NativeRandomDeallocator = org.nd4j.rng.deallocator.NativeRandomDeallocator
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
'ORIGINAL LINE: @ThreadLeakFilters(defaultFilters = true, filters = { ModelTupleStreamIntegrationTest.PrivateDeallocatorThreadsFilter.class }) @DisplayName("Model Tuple Stream Integration Test") @Disabled("Timeout issue") @Tag(TagNames.SOLR) @Tag(TagNames.DIST_SYSTEMS) class ModelTupleStreamIntegrationTest extends org.apache.solr.cloud.SolrCloudTestCase
	Friend Class ModelTupleStreamIntegrationTest
		Inherits SolrCloudTestCase

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

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Private Deallocator Threads Filter") static class PrivateDeallocatorThreadsFilter implements com.carrotsearch.randomizedtesting.ThreadFilter
		Friend Class PrivateDeallocatorThreadsFilter
			Implements ThreadFilter

			Public Overrides Function reject(ByVal thread As Thread) As Boolean
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final ThreadGroup threadGroup = thread.getThreadGroup();
				Dim threadGroup As ThreadGroup = thread.getThreadGroup()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String threadGroupName = (threadGroup == null ? null : threadGroup.getName());
				Dim threadGroupName As String = (If(threadGroup Is Nothing, Nothing, threadGroup.getName()))
				If threadGroupName IsNot Nothing AndAlso threadGroupName.EndsWith(GetType(ModelTupleStreamIntegrationTest).Name, StringComparison.Ordinal) Then
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String threadName = thread.getName();
					Dim threadName As String = thread.getName()
					If threadName.StartsWith(NativeRandomDeallocator.DeallocatorThreadNamePrefix, StringComparison.Ordinal) OrElse threadName.ToLower().Contains("deallocator") OrElse threadName.Equals(BasicWorkspaceManager.WorkspaceDeallocatorThreadName) Then
						Return True
					End If
				End If
				Return False
			End Function
		End Class

		Private Const MY_COLLECTION_NAME As String = "mySolrCollection"

		Private Const MY_SERIALIZED_MODEL_FILENAME As String = "mySerializedModel"

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeAll static void setupCluster() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Shared Sub setupCluster()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.nio.file.Path configsetPath = configset("mini-expressible");
			Dim configsetPath As Path = configset("mini-expressible")
			' create and serialize model
			If True Then
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.nn.api.Model model = buildModel();
				Dim model As Model = buildModel()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.io.File serializedModelFile = configsetPath.resolve(MY_SERIALIZED_MODEL_FILENAME).toFile();
				Dim serializedModelFile As File = configsetPath.resolve(MY_SERIALIZED_MODEL_FILENAME).toFile()
				ModelSerializer.writeModel(model, serializedModelFile.getPath(), False)
			End If
			Const configName As String = "conf"
			Const numShards As Integer = 2
			Const numReplicas As Integer = 2
			Const maxShardsPerNode As Integer = 1
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int nodeCount = (numShards * numReplicas + (maxShardsPerNode - 1)) / maxShardsPerNode;
			Dim nodeCount As Integer = (numShards * numReplicas + (maxShardsPerNode - 1)) \ maxShardsPerNode
			' create and configure cluster
			configureCluster(nodeCount).addConfig(configName, configsetPath).configure()
			' create an empty collection
			CollectionAdminRequest.createCollection(MY_COLLECTION_NAME, configName, numShards, numReplicas).setMaxShardsPerNode(maxShardsPerNode).process(cluster.getSolrClient())
			' compose an update request
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.apache.solr.client.solrj.request.UpdateRequest updateRequest = new org.apache.solr.client.solrj.request.UpdateRequest();
			Dim updateRequest As New UpdateRequest()
			' add some documents
			updateRequest.add(sdoc("id", "green", "channel_b_f", "0", "channel_g_f", "255", "channel_r_f", "0"))
			updateRequest.add(sdoc("id", "black", "channel_b_f", "0", "channel_g_f", "0", "channel_r_f", "0"))
			updateRequest.add(sdoc("id", "yellow", "channel_b_f", "0", "channel_g_f", "255", "channel_r_f", "255"))
			' make the update request
			updateRequest.commit(cluster.getSolrClient(), MY_COLLECTION_NAME)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static org.deeplearning4j.nn.api.Model buildModel() throws Exception
		Private Shared Function buildModel() As Model
			Const numInputs As Integer = 3
			Const numOutputs As Integer = 2
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.nn.conf.MultiLayerConfiguration conf = new org.deeplearning4j.nn.conf.NeuralNetConfiguration.Builder().list(new org.deeplearning4j.nn.conf.layers.OutputLayer.Builder().nIn(numInputs).nOut(numOutputs).activation(org.nd4j.linalg.activations.Activation.IDENTITY).lossFunction(org.nd4j.linalg.lossfunctions.LossFunctions.LossFunction.MSE).build()).build();
			Dim conf As MultiLayerConfiguration = (New NeuralNetConfiguration.Builder()).list((New OutputLayer.Builder()).nIn(numInputs).nOut(numOutputs).activation(Activation.IDENTITY).lossFunction(LossFunctions.LossFunction.MSE).build()).build()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.nn.multilayer.MultiLayerNetwork model = new org.deeplearning4j.nn.multilayer.MultiLayerNetwork(conf);
			Dim model As New MultiLayerNetwork(conf)
			model.init()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final float[] floats = new float[] { +1, +1, +1, -1, -1, -1, 0, 0 };
			Dim floats() As Single = { +1, +1, +1, -1, -1, -1, 0, 0 }
			' positive weight for first output, negative weight for second output, no biases
			assertEquals((numInputs + 1) * numOutputs, floats.Length)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.api.ndarray.INDArray params = org.nd4j.linalg.factory.Nd4j.create(floats);
			Dim params As INDArray = Nd4j.create(floats)
			model.Params = params
			Return model
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void doTest(String expr, String[] expectedIds, Object[] expectedLefts, Object[] expectedRights) throws Exception
		Private Sub doTest(ByVal expr As String, ByVal expectedIds() As String, ByVal expectedLefts() As Object, ByVal expectedRights() As Object)
			Dim paramsLoc As New ModifiableSolrParams()
			paramsLoc.set("expr", expr)
			paramsLoc.set("qt", "/stream")
			Dim url As String = cluster.getRandomJetty(random()).getBaseUrl().ToString() & "/" & MY_COLLECTION_NAME
			Dim tupleStream As TupleStream = New SolrStream(url, paramsLoc)
			Dim context As New StreamContext()
			tupleStream.setStreamContext(context)
			Try
				tupleStream.open()
				For ii As Integer = 0 To expectedIds.Length - 1
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.apache.solr.client.solrj.io.Tuple tuple = tupleStream.read();
					Dim tuple As Tuple = tupleStream.read()
					assertFalse(tuple.EOF)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String expectedId = expectedIds[ii];
					Dim expectedId As String = expectedIds(ii)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String actualId = tuple.getString("id");
					Dim actualId As String = tuple.getString("id")
					assertEquals(expectedId, actualId)
					If expectedLefts IsNot Nothing Then
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final Object expectedLeft = expectedLefts[ii];
						Dim expectedLeft As Object = expectedLefts(ii)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String actualLeft = tuple.getString("left");
						Dim actualLeft As String = tuple.getString("left")
						assertEquals(tuple.getMap().ToString(), expectedLeft, actualLeft)
					End If
					If expectedRights IsNot Nothing Then
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final Object expectedRight = expectedRights[ii];
						Dim expectedRight As Object = expectedRights(ii)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String actualRight = tuple.getString("right");
						Dim actualRight As String = tuple.getString("right")
						assertEquals(tuple.getMap().ToString(), expectedRight, actualRight)
					End If
				Next ii
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.apache.solr.client.solrj.io.Tuple lastTuple = tupleStream.read();
				Dim lastTuple As Tuple = tupleStream.read()
				assertTrue(lastTuple.EOF)
			Finally
				tupleStream.close()
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Search Test") void searchTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub searchTest()
			Dim testsCount As Integer = 0
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String searchExpr = "search(" + MY_COLLECTION_NAME + "," + "zkHost=""" + cluster.getZkClient().getZkServerAddress() + """," + "q=""*:*""," + "fl=""id,channel_b_f,channel_g_f,channel_r_f""," + "qt=""/export""," + "sort=""id asc"")";
			Dim searchExpr As String = "search(" & MY_COLLECTION_NAME & "," & "zkHost=""" & cluster.getZkClient().getZkServerAddress() & """," & "q=""*:*""," & "fl=""id,channel_b_f,channel_g_f,channel_r_f""," & "qt=""/export""," & "sort=""id asc"")"
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String modelTupleExpr = "modelTuple(" + searchExpr + "," + "serializedModelFileName=""" + MY_SERIALIZED_MODEL_FILENAME + """," + "inputKeys=""channel_b_f,channel_g_f,channel_r_f""," + "outputKeys=""left,right"")";
			Dim modelTupleExpr As String = "modelTuple(" & searchExpr & "," & "serializedModelFileName=""" & MY_SERIALIZED_MODEL_FILENAME & """," & "inputKeys=""channel_b_f,channel_g_f,channel_r_f""," & "outputKeys=""left,right"")"
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String[] expectedIds = new String[] { "black", "green", "yellow" };
			Dim expectedIds() As String = { "black", "green", "yellow" }
			If True Then
				Const expectedLefts() As String = Nothing
				Const expectedRights() As String = Nothing
				doTest(searchExpr, expectedIds, expectedLefts, expectedRights)
				testsCount += 1
			End If
			If True Then
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String[] expectedLefts = new String[] { "0.0", "255.0", "510.0" };
				Dim expectedLefts() As String = { "0.0", "255.0", "510.0" }
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final String[] expectedRights = new String[] { "0.0", "-255.0", "-510.0" };
				Dim expectedRights() As String = { "0.0", "-255.0", "-510.0" }
				doTest(modelTupleExpr, expectedIds, expectedLefts, expectedRights)
				testsCount += 1
			End If
			assertEquals(2, testsCount)
		End Sub
	End Class

End Namespace