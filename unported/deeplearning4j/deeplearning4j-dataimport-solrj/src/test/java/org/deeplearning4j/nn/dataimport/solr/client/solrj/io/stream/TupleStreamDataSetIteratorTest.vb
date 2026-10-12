Imports System
Imports System.Collections.Generic
Imports System.Threading
Imports ThreadLeakFilters = com.carrotsearch.randomizedtesting.annotations.ThreadLeakFilters
Imports ThreadFilter = com.carrotsearch.randomizedtesting.ThreadFilter
Imports CollectionAdminRequest = org.apache.solr.client.solrj.request.CollectionAdminRequest
Imports UpdateRequest = org.apache.solr.client.solrj.request.UpdateRequest
Imports SolrCloudTestCase = org.apache.solr.cloud.SolrCloudTestCase
Imports SolrInputDocument = org.apache.solr.common.SolrInputDocument
Imports Model = org.deeplearning4j.nn.api.Model
Imports NeuralNetConfiguration = org.deeplearning4j.nn.conf.NeuralNetConfiguration
Imports OutputLayer = org.deeplearning4j.nn.conf.layers.OutputLayer
Imports MultiLayerNetwork = org.deeplearning4j.nn.multilayer.MultiLayerNetwork
Imports WeightInit = org.deeplearning4j.nn.weights.WeightInit
Imports ScoreIterationListener = org.deeplearning4j.optimize.listeners.ScoreIterationListener
Imports org.junit.jupiter.api
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports Activation = org.nd4j.linalg.activations.Activation
Imports DataSet = org.nd4j.linalg.dataset.DataSet
Imports LossFunction = org.nd4j.linalg.lossfunctions.LossFunctions.LossFunction
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
Namespace org.deeplearning4j.nn.dataimport.solr.client.solrj.io.stream

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ThreadLeakFilters(defaultFilters = true, filters = { TupleStreamDataSetIteratorTest.PrivateDeallocatorThreadsFilter.class }) @DisplayName("Tuple Stream Data Set Iterator Test") @Tag(TagNames.SOLR) @Tag(TagNames.DIST_SYSTEMS) class TupleStreamDataSetIteratorTest extends org.apache.solr.cloud.SolrCloudTestCase
	Friend Class TupleStreamDataSetIteratorTest
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
				If threadGroupName IsNot Nothing AndAlso threadGroupName.EndsWith(GetType(TupleStreamDataSetIteratorTest).Name, StringComparison.Ordinal) Then
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

		Private Shared numDocs As Integer = 0

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeAll static void setupCluster() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Shared Sub setupCluster()
			Const numShards As Integer = 2
			Const numReplicas As Integer = 2
			Const maxShardsPerNode As Integer = 1
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final int nodeCount = (numShards * numReplicas + (maxShardsPerNode - 1)) / maxShardsPerNode;
			Dim nodeCount As Integer = (numShards * numReplicas + (maxShardsPerNode - 1)) \ maxShardsPerNode
			' create and configure cluster
			configureCluster(nodeCount).addConfig("conf", configset("mini")).configure()
			' create an empty collection
			CollectionAdminRequest.createCollection("mySolrCollection", "conf", numShards, numReplicas).setMaxShardsPerNode(maxShardsPerNode).process(cluster.getSolrClient())
			' compose an update request
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.apache.solr.client.solrj.request.UpdateRequest updateRequest = new org.apache.solr.client.solrj.request.UpdateRequest();
			Dim updateRequest As New UpdateRequest()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.List<Integer> docIds = new java.util.ArrayList<>();
			Dim docIds As IList(Of Integer) = New List(Of Integer)()
			For phase As Integer = 1 To 2
				Dim docIdsIdx As Integer = 0
				If phase = 2 Then
					Collections.shuffle(docIds)
				End If
				Const increment As Integer = 32
				For b As Integer = 0 To 256 Step increment
					If 256 = b Then
						b -= 1
					End If
					For g As Integer = 0 To 256 Step increment
						If 256 = g Then
							g -= 1
						End If
						For r As Integer = 0 To 256 Step increment
							If 256 = r Then
								r -= 1
							End If
							If phase = 1 Then
								docIds.Add(docIds.Count + 1)
								Continue For
							End If
							' https://en.wikipedia.org/wiki/Luma_(video)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final float luminance = (b * 0.0722f + g * 0.7152f + r * 0.2126f) / (255 * 3.0f);
							Dim luminance As Single = (b * 0.0722f + g * 0.7152f + r * 0.2126f) / (255 * 3.0f)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.apache.solr.common.SolrInputDocument doc = sdoc("id", System.Convert.ToString(docIds.get(docIdsIdx++)), "channel_b_f", System.Convert.ToString(b / 255f), "channel_g_f", System.Convert.ToString(g / 255f), "channel_r_f", System.Convert.ToString(r / 255f), "luminance_f", System.Convert.ToString(luminance));
'JAVA TO VB CONVERTER WARNING: An assignment within expression was extracted from the following statement:
							Dim doc As SolrInputDocument = sdoc("id", Convert.ToString(docIds(docIdsIdx)), "channel_b_f", Convert.ToString(b / 255f), "channel_g_f", Convert.ToString(g / 255f), "channel_r_f", Convert.ToString(r / 255f), "luminance_f", Convert.ToString(luminance))
								docIdsIdx += 1
							updateRequest.add(doc)
							numDocs += 1
						Next r
					Next g
				Next b
			Next phase
			' make the update request
			updateRequest.commit(cluster.getSolrClient(), "mySolrCollection")
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @DisplayName("Counting Iteration Listener") private static class CountingIterationListener extends org.deeplearning4j.optimize.listeners.ScoreIterationListener
		<Serializable>
		Private Class CountingIterationListener
			Inherits ScoreIterationListener

'JAVA TO VB CONVERTER NOTE: The field numIterationsDone was renamed since Visual Basic does not allow fields to have the same name as other class members:
			Friend numIterationsDone_Conflict As Integer = 0

			Public Sub New()
				MyBase.New(1)
			End Sub

			Public Overridable Function numIterationsDone() As Integer
				Return numIterationsDone_Conflict
			End Function

			Public Overrides Sub iterationDone(ByVal model As Model, ByVal iteration As Integer, ByVal epoch As Integer)
				MyBase.iterationDone(model, iteration, epoch)
				numIterationsDone_Conflict += 1
			End Sub
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Iterate Test") void iterateTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub iterateTest()
			doIterateTest(True)
			doIterateTest(False)
		End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private void doIterateTest(boolean withIdKey) throws Exception
		Private Sub doIterateTest(ByVal withIdKey As Boolean)
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: try (final TupleStreamDataSetIterator tsdsi = new TupleStreamDataSetIterator(123, (withIdKey ? "greeting" : null), new String[] { "pie" }, new String[] { "answer" }, "tuple(greeting=""hello world"",pie=3.14,answer=42)", null))
			Using tsdsi As New TupleStreamDataSetIterator(123, (If(withIdKey, "greeting", Nothing)), New String() { "pie" }, New String() { "answer" }, "tuple(greeting=""hello world"",pie=3.14,answer=42)", Nothing)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				assertTrue(tsdsi.hasNext())
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.nd4j.linalg.dataset.DataSet ds = tsdsi.next();
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				Dim ds As DataSet = tsdsi.next()
				assertEquals(1, ds.Features.length())
				assertEquals(3.14f, ds.Features.getFloat(0), 0.0f)
				assertEquals(1, ds.Labels.length())
				assertEquals(42f, ds.Labels.getFloat(0), 0.0f)
'JAVA TO VB CONVERTER TODO TASK: Java iterators are only converted within the context of 'while' and 'for' loops:
				assertFalse(tsdsi.hasNext())
			End Using
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @DisplayName("Model Fit Test") void modelFitTest() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Friend Overridable Sub modelFitTest()
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final org.deeplearning4j.nn.multilayer.MultiLayerNetwork model = new org.deeplearning4j.nn.multilayer.MultiLayerNetwork(new org.deeplearning4j.nn.conf.NeuralNetConfiguration.Builder().list(new org.deeplearning4j.nn.conf.layers.OutputLayer.Builder(org.nd4j.linalg.lossfunctions.LossFunctions.LossFunction.MSE).nIn(3).nOut(1).weightInit(org.deeplearning4j.nn.weights.WeightInit.ONES).activation(org.nd4j.linalg.activations.Activation.IDENTITY).build()).build());
			Dim model As New MultiLayerNetwork((New NeuralNetConfiguration.Builder()).list((New OutputLayer.Builder(LossFunction.MSE)).nIn(3).nOut(1).weightInit(WeightInit.ONES).activation(Activation.IDENTITY).build()).build())
			model.init()
			Dim batch As Integer = 1
			For ii As Integer = 1 To 5
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final CountingIterationListener listener = new CountingIterationListener();
				Dim listener As New CountingIterationListener()
				model.setListeners(listener)
				batch *= 2
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: try (final TupleStreamDataSetIterator tsdsi = new TupleStreamDataSetIterator(batch, "id", new String[] { "channel_b_f", "channel_g_f", "channel_r_f" }, new String[] { "luminance_f" }, "search(mySolrCollection," + "q=""id:*""," + "fl=""id,channel_b_f,channel_g_f,channel_r_f,luminance_f""," + "sort=""id asc""," + "qt=""/export"")", cluster.getZkClient().getZkServerAddress()))
				Using tsdsi As New TupleStreamDataSetIterator(batch, "id", New String() { "channel_b_f", "channel_g_f", "channel_r_f" }, New String() { "luminance_f" }, "search(mySolrCollection," & "q=""id:*""," & "fl=""id,channel_b_f,channel_g_f,channel_r_f,luminance_f""," & "sort=""id asc""," & "qt=""/export"")", cluster.getZkClient().getZkServerAddress())
					model.fit(tsdsi)
				End Using
				assertEquals("numIterationsDone=" & listener.numIterationsDone() & " numDocs=" & numDocs & " batch=" & batch, (numDocs + (batch - 1)) \ batch, listener.numIterationsDone())
			Next ii
		End Sub
	End Class

End Namespace