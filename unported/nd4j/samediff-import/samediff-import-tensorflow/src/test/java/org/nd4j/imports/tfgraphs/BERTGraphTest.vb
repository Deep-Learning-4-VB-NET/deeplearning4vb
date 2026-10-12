Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports TrainingConfig = org.nd4j.autodiff.samediff.TrainingConfig
Imports GraphTransformUtil = org.nd4j.autodiff.samediff.transform.GraphTransformUtil
Imports OpPredicate = org.nd4j.autodiff.samediff.transform.OpPredicate
Imports SubGraph = org.nd4j.autodiff.samediff.transform.SubGraph
Imports SubGraphPredicate = org.nd4j.autodiff.samediff.transform.SubGraphPredicate
Imports SubGraphProcessor = org.nd4j.autodiff.samediff.transform.SubGraphProcessor
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports LogFileWriter = org.nd4j.graph.ui.LogFileWriter
Imports TFGraphMapper = org.nd4j.imports.graphmapper.tf.TFGraphMapper
Imports TFImportOverride = org.nd4j.imports.tensorflow.TFImportOverride
Imports TFOpImportFilter = org.nd4j.imports.tensorflow.TFOpImportFilter
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports MultiDataSet = org.nd4j.linalg.dataset.api.MultiDataSet
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports Downloader = org.nd4j.common.resources.Downloader
Imports ArchiveUtils = org.nd4j.common.util.ArchiveUtils
import static org.junit.jupiter.api.Assertions.assertEquals
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

Namespace org.nd4j.imports.tfgraphs


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Tag(TagNames.LONG_TEST) @Tag(TagNames.LARGE_RESOURCES) public class BERTGraphTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class BERTGraphTest
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBert(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testBert(ByVal backend As Nd4jBackend)

			Dim url As String = "https://dl4jdata.blob.core.windows.net/testresources/bert_mrpc_frozen_v1.zip"
			Dim saveDir As New File(TFGraphTestZooModels.BaseModelDir, ".nd4jtests/bert_mrpc_frozen_v1")
			saveDir.mkdirs()

			Dim localFile As New File(saveDir, "bert_mrpc_frozen_v1.zip")
			Dim md5 As String = "7cef8bbe62e701212472f77a0361f443"

			If localFile.exists() AndAlso Not Downloader.checkMD5OfFile(md5, localFile) Then
				log.info("Deleting local file: does not match MD5. {}", localFile.getAbsolutePath())
				localFile.delete()
			End If

			If Not localFile.exists() Then
				log.info("Starting resource download from: {} to {}", url, localFile.getAbsolutePath())
				Downloader.download("BERT MRPC", New URL(url), localFile, md5, 3)
			End If

			'Extract
			Dim f As New File(saveDir, "bert_mrpc_frozen.pb")
			If Not f.exists() OrElse Not Downloader.checkMD5OfFile("93d82bca887625632578df37ea3d3ca5", f) Then
				If f.exists() Then
					f.delete()
				End If
				ArchiveUtils.zipExtractSingleFile(localFile, f, "bert_mrpc_frozen.pb")
			End If

	'        
	'        Important node: This BERT model uses a FIXED (hardcoded) minibatch size, not dynamic as most models use
	'         
			Dim minibatchSize As Integer = 4

	'        
	'         * Define: Op import overrides. This is used to skip the IteratorGetNext node and instead crate some placeholders
	'         
			Dim m As IDictionary(Of String, TFImportOverride) = New Dictionary(Of String, TFImportOverride)()
			m("IteratorGetNext") = Function(inputs, controlDepInputs, nodeDef, initWith, attributesForNode, graph)
			Return java.util.Arrays.asList(initWith.placeHolder("IteratorGetNext", DataType.INT, minibatchSize, 128), initWith.placeHolder("IteratorGetNext:1", DataType.INT, minibatchSize, 128), initWith.placeHolder("IteratorGetNext:4", DataType.INT, minibatchSize, 128))
			End Function

			'Skip the "IteratorV2" op - we don't want or need this
			Dim filter As TFOpImportFilter = Function(nodeDef, initWith, attributesForNode, graph)
			Return "IteratorV2".Equals(nodeDef.getName())
			End Function

			Dim sd As SameDiff = TFGraphMapper.importGraph(f, m, filter)

	'        
	'        Modify the network to remove hard-coded dropout operations for inference.
	'        This is a little ugly as Tensorflow/BERT's dropout is implemented as a set of discrete operations - random, mul, div, floor, etc.
	'        We need to select all instances of this subgraph, and then remove them from the graph entirely.
	'
	'        Note that in general there are two ways to define subgraphs (larger than 1 operation) for use in GraphTransformUtil
	'        (a) withInputSubgraph - the input must match this predicate, AND it is added to the subgraph (i.e., matched and is selected to be part of the subgraph)
	'        (b) withInputMatching - the input must match this predicate, BUT it is NOT added to the subgraph (i.e., must match only)
	'
	'        In effect, this predicate will match the set of directly connected operations with the following structure:
	'        (.../dropout/div, .../dropout/Floor) -> (.../dropout/mul)
	'        (.../dropout/add) -> (.../dropout/Floor)
	'        (.../dropout/random_uniform) -> (.../dropout/add)
	'        (.../dropout/random_uniform/mul) -> (.../dropout/random_uniform)
	'        (.../dropout/random_uniform/RandomUniform, .../dropout/random_uniform/sub) -> (.../dropout/random_uniform/mul)
	'
	'        Then, for all subgraphs that match this predicate, we will process them (in this case, simply replace the entire subgraph by passing the input to the output)
	'
	'        How do you work out the appropriate subgraph to replace?
	'        The simplest approach is to visualize the graph - either in TensorBoard or using SameDiff UI.
	'        See writeBertUI() in this file, then open DL4J UI and go to localhost:9000/samediff
	'        
			Dim p As SubGraphPredicate = SubGraphPredicate.withRoot(OpPredicate.nameMatches(".*/dropout/mul")).withInputCount(2).withInputSubgraph(0, SubGraphPredicate.withRoot(OpPredicate.nameMatches(".*/dropout/div"))).withInputSubgraph(1, SubGraphPredicate.withRoot(OpPredicate.nameMatches(".*/dropout/Floor")).withInputSubgraph(0, SubGraphPredicate.withRoot(OpPredicate.nameMatches(".*/dropout/add")).withInputSubgraph(1, SubGraphPredicate.withRoot(OpPredicate.nameMatches(".*/dropout/random_uniform")).withInputSubgraph(0, SubGraphPredicate.withRoot(OpPredicate.nameMatches(".*/dropout/random_uniform/mul")).withInputSubgraph(0, SubGraphPredicate.withRoot(OpPredicate.nameMatches(".*/dropout/random_uniform/RandomUniform"))).withInputSubgraph(1, SubGraphPredicate.withRoot(OpPredicate.nameMatches(".*/dropout/random_uniform/sub")))))))

			Dim subGraphs As IList(Of SubGraph) = GraphTransformUtil.getSubgraphsMatching(sd, p)
			Dim subGraphCount As Integer = subGraphs.Count
			assertTrue(subGraphCount > 0,"Subgraph count: " & subGraphCount)


	'        
	'        Create the subgraph processor.
	'        The subgraph processor is applied to each subgraph - i.e., it defines what we should replace it with.
	'        It's a 2-step process:
	'        (1) The SubGraphProcessor is applied to define the replacement subgraph (add any new operations, and define the new outputs, etc).
	'            In this case, we aren't adding any new ops - so we'll just pass the "real" input (pre dropout activations) to the output.
	'            Note that the number of returned outputs must match the existing number of outputs (1 in this case).
	'            Immediately after SubgraphProcessor.processSubgraph returns, both the existing subgraph (to be replaced) and new subgraph (just added)
	'            exist in parallel.
	'        (2) The existing subgraph is then removed from the graph, leaving only the new subgraph (as defined in processSubgraph method)
	'            in its place.
	'
	'         Note that the order of the outputs you return matters!
	'         If the original outputs are [A,B,C] and you return output variables [X,Y,Z], then anywhere "A" was used as input
	'         will now use "X"; similarly Y replaces B, and Z replaces C.
	'         
			sd = GraphTransformUtil.replaceSubgraphsMatching(sd, p, New SubGraphProcessorAnonymousInnerClass(Me))

			'Small test / sanity check for asFlatPrint():
			sd.asFlatPrint()


	'        
	'        Output during inference:
	'        INFO:tensorflow:*** Example ***
	'        INFO:tensorflow:guid: test-1
	'        INFO:tensorflow:tokens: [CLS] the broader standard & poor ' s 500 index < . sp ##x > was 0 . 46 points lower , or 0 . 05 percent , at 99 ##7 . 02 . [SEP] the technology - laced nas ##da ##q composite index . ix ##ic was up 7 . 42 points , or 0 . 45 percent , at 1 , 65 ##3 . 44 . [SEP]
	'        INFO:tensorflow:input_ids: 101 1996 12368 3115 1004 3532 1005 1055 3156 5950 1026 1012 11867 2595 1028 2001 1014 1012 4805 2685 2896 1010 2030 1014 1012 5709 3867 1010 2012 5585 2581 1012 6185 1012 102 1996 2974 1011 17958 17235 2850 4160 12490 5950 1012 11814 2594 2001 2039 1021 1012 4413 2685 1010 2030 1014 1012 3429 3867 1010 2012 1015 1010 3515 2509 1012 4008 1012 102 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
	'        INFO:tensorflow:input_mask: 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
	'        INFO:tensorflow:segment_ids: 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
	'        INFO:tensorflow:label: 0 (id = 0)
	'        INFO:tensorflow:*** Example ***
	'        INFO:tensorflow:guid: test-2
	'        INFO:tensorflow:tokens: [CLS] shares in ba were down 1 . 5 percent at 168 pen ##ce by 142 ##0 gm ##t , off a low of 164 ##p , in a slightly stronger overall london market . [SEP] shares in ba were down three percent at 165 - 1 / 4 pen ##ce by 09 ##33 gm ##t , off a low of 164 pen ##ce , in a stronger market . [SEP]
	'        INFO:tensorflow:input_ids: 101 6661 1999 8670 2020 2091 1015 1012 1019 3867 2012 16923 7279 3401 2011 16087 2692 13938 2102 1010 2125 1037 2659 1997 17943 2361 1010 1999 1037 3621 6428 3452 2414 3006 1012 102 6661 1999 8670 2020 2091 2093 3867 2012 13913 1011 1015 1013 1018 7279 3401 2011 5641 22394 13938 2102 1010 2125 1037 2659 1997 17943 7279 3401 1010 1999 1037 6428 3006 1012 102 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
	'        INFO:tensorflow:input_mask: 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
	'        INFO:tensorflow:segment_ids: 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
	'        INFO:tensorflow:label: 0 (id = 0)
	'        INFO:tensorflow:*** Example ***
	'        INFO:tensorflow:guid: test-3
	'        INFO:tensorflow:tokens: [CLS] last year , com ##cast signed 1 . 5 million new digital cable subscribers . [SEP] com ##cast has about 21 . 3 million cable subscribers , many in the largest u . s . cities . [SEP]
	'        INFO:tensorflow:input_ids: 101 2197 2095 1010 4012 10526 2772 1015 1012 1019 2454 2047 3617 5830 17073 1012 102 4012 10526 2038 2055 2538 1012 1017 2454 5830 17073 1010 2116 1999 1996 2922 1057 1012 1055 1012 3655 1012 102 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
	'        INFO:tensorflow:input_mask: 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
	'        INFO:tensorflow:segment_ids: 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
	'        INFO:tensorflow:label: 0 (id = 0)
	'        INFO:tensorflow:*** Example ***
	'        INFO:tensorflow:guid: test-4
	'        INFO:tensorflow:tokens: [CLS] revenue rose 3 . 9 percent , to $ 1 . 63 billion from $ 1 . 57 billion . [SEP] the mclean , virginia - based company said newspaper revenue increased 5 percent to $ 1 . 46 billion . [SEP]
	'        INFO:tensorflow:input_ids: 101 6599 3123 1017 1012 1023 3867 1010 2000 1002 1015 1012 6191 4551 2013 1002 1015 1012 5401 4551 1012 102 1996 17602 1010 3448 1011 2241 2194 2056 3780 6599 3445 1019 3867 2000 1002 1015 1012 4805 4551 1012 102 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
	'        INFO:tensorflow:input_mask: 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
	'        INFO:tensorflow:segment_ids: 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
	'        INFO:tensorflow:label: 0 (id = 0)
	'         
			Dim ex1Idxs As INDArray = Nd4j.createFromArray(101,1996,12368,3115,1004,3532,1005,1055,3156,5950,1026,1012,11867,2595,1028,2001,1014,1012,4805,2685,2896,1010,2030,1014,1012,5709,3867,1010,2012,5585,2581,1012,6185,1012,102,1996,2974,1011,17958,17235,2850,4160,12490,5950,1012,11814,2594,2001,2039,1021,1012,4413,2685,1010,2030,1014,1012,3429,3867,1010,2012,1015,1010,3515,2509,1012,4008,1012,102,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0)
			Dim ex1Mask As INDArray = Nd4j.createFromArray(1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0)
			Dim ex1SegmentId As INDArray = Nd4j.createFromArray(0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0)

			Dim ex2Idxs As INDArray = Nd4j.createFromArray(101,6661,1999,8670,2020,2091,1015,1012,1019,3867,2012,16923,7279,3401,2011,16087,2692,13938,2102,1010,2125,1037,2659,1997,17943,2361,1010,1999,1037,3621,6428,3452,2414,3006,1012,102,6661,1999,8670,2020,2091,2093,3867,2012,13913,1011,1015,1013,1018,7279,3401,2011,5641,22394,13938,2102,1010,2125,1037,2659,1997,17943,7279,3401,1010,1999,1037,6428,3006,1012,102,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0)
			Dim ex2Mask As INDArray = Nd4j.createFromArray(1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0)
			Dim ex2SegmentId As INDArray = Nd4j.createFromArray(0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0)

			Dim ex3Idxs As INDArray = Nd4j.createFromArray(101,2197,2095,1010,4012,10526,2772,1015,1012,1019,2454,2047,3617,5830,17073,1012,102,4012,10526,2038,2055,2538,1012,1017,2454,5830,17073,1010,2116,1999,1996,2922,1057,1012,1055,1012,3655,1012,102,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0)
			Dim ex3Mask As INDArray = Nd4j.createFromArray(1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0)
			Dim ex3SegmentId As INDArray = Nd4j.createFromArray(0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0)

			Dim ex4Idxs As INDArray = Nd4j.createFromArray(101,6599,3123,1017,1012,1023,3867,1010,2000,1002,1015,1012,6191,4551,2013,1002,1015,1012,5401,4551,1012,102,1996,17602,1010,3448,1011,2241,2194,2056,3780,6599,3445,1019,3867,2000,1002,1015,1012,4805,4551,1012,102,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0)
			Dim ex4Mask As INDArray = Nd4j.createFromArray(1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0)
			Dim ex4SegmentId As INDArray = Nd4j.createFromArray(0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0)

			Dim idxs As INDArray = Nd4j.vstack(ex1Idxs, ex2Idxs, ex3Idxs, ex4Idxs)
			Dim mask As INDArray = Nd4j.vstack(ex1Mask, ex2Mask, ex3Mask, ex4Mask)
			Dim segmentIdxs As INDArray = Nd4j.vstack(ex1SegmentId, ex2SegmentId, ex3SegmentId, ex4SegmentId)

			Dim placeholderValues As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			placeholderValues("IteratorGetNext") = idxs
			placeholderValues("IteratorGetNext:1") = mask
			placeholderValues("IteratorGetNext:4") = segmentIdxs

			Dim [out] As IDictionary(Of String, INDArray) = sd.output(placeholderValues, "loss/Softmax")
			Dim softmax As INDArray = [out]("loss/Softmax")
	'        System.out.println("OUTPUT - Softmax");
	'        System.out.println(softmax);
	'        System.out.println(Arrays.toString(softmax.data().asFloat()));

			Dim exp0 As INDArray = Nd4j.createFromArray(0.99860954f, 0.0013904407f)
			Dim exp1 As INDArray = Nd4j.createFromArray(0.0005442508f, 0.99945575f)
			Dim exp2 As INDArray = Nd4j.createFromArray(0.9987967f, 0.0012033002f)
			Dim exp3 As INDArray = Nd4j.createFromArray(0.97409827f, 0.025901746f)

			assertEquals(exp0, softmax.getRow(0))
			assertEquals(exp1, softmax.getRow(1))
			assertEquals(exp2, softmax.getRow(2))
			assertEquals(exp3, softmax.getRow(3))
		End Sub

		Private Class SubGraphProcessorAnonymousInnerClass
			Implements SubGraphProcessor

			Private ReadOnly outerInstance As BERTGraphTest

			Public Sub New(ByVal outerInstance As BERTGraphTest)
				Me.outerInstance = outerInstance
			End Sub

			Public Function processSubgraph(ByVal sd As SameDiff, ByVal subGraph As SubGraph) As IList(Of SDVariable)
				Dim inputs As IList(Of SDVariable) = subGraph.inputs() 'Get inputs to the subgraph
				'Find pre-dropout input variable:
				Dim newOut As SDVariable = Nothing
				For Each v As SDVariable In inputs
					If v.name().EndsWith("/BiasAdd", StringComparison.Ordinal) OrElse v.name().EndsWith("/Softmax", StringComparison.Ordinal) OrElse v.name().EndsWith("/add_1", StringComparison.Ordinal) OrElse v.name().EndsWith("/Tanh", StringComparison.Ordinal) Then
						newOut = v
						Exit For
					End If
				Next v

				If newOut IsNot Nothing Then
					'Pass this input variable as the new output
					Return Collections.singletonList(newOut)
				End If

				Throw New Exception("No pre-dropout input variable found")
			End Function
		End Class

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBertTraining(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testBertTraining(ByVal backend As Nd4jBackend)
			Dim url As String = "https://dl4jdata.blob.core.windows.net/testresources/bert_mrpc_frozen_v1.zip"
			Dim saveDir As New File(TFGraphTestZooModels.BaseModelDir, ".nd4jtests/bert_mrpc_frozen_v1")
			saveDir.mkdirs()

			Dim localFile As New File(saveDir, "bert_mrpc_frozen_v1.zip")
			Dim md5 As String = "7cef8bbe62e701212472f77a0361f443"

			If localFile.exists() AndAlso Not Downloader.checkMD5OfFile(md5, localFile) Then
				log.info("Deleting local file: does not match MD5. {}", localFile.getAbsolutePath())
				localFile.delete()
			End If

			If Not localFile.exists() Then
				log.info("Starting resource download from: {} to {}", url, localFile.getAbsolutePath())
				Downloader.download("BERT MRPC", New URL(url), localFile, md5, 3)
			End If

			'Extract
			Dim f As New File(saveDir, "bert_mrpc_frozen.pb")
			If Not f.exists() OrElse Not Downloader.checkMD5OfFile("93d82bca887625632578df37ea3d3ca5", f) Then
				If f.exists() Then
					f.delete()
				End If
				ArchiveUtils.zipExtractSingleFile(localFile, f, "bert_mrpc_frozen.pb")
			End If

	'        
	'        Important node: This BERT model uses a FIXED (hardcoded) minibatch size, not dynamic as most models use
	'         
			Dim minibatchSize As Integer = 4

	'        
	'         * Define: Op import overrides. This is used to skip the IteratorGetNext node and instead crate some placeholders
	'         
			Dim m As IDictionary(Of String, TFImportOverride) = New Dictionary(Of String, TFImportOverride)()
			m("IteratorGetNext") = Function(inputs, controlDepInputs, nodeDef, initWith, attributesForNode, graph)
			Return java.util.Arrays.asList(initWith.placeHolder("IteratorGetNext", DataType.INT, minibatchSize, 128), initWith.placeHolder("IteratorGetNext:1", DataType.INT, minibatchSize, 128), initWith.placeHolder("IteratorGetNext:4", DataType.INT, minibatchSize, 128))
			End Function

			'Skip the "IteratorV2" op - we don't want or need this
			Dim filter As TFOpImportFilter = Function(nodeDef, initWith, attributesForNode, graph)
			Return "IteratorV2".Equals(nodeDef.getName())
			End Function

			Dim sd As SameDiff = TFGraphMapper.importGraph(f, m, filter)

	'        
	'        Set<String> floatConstants = new HashSet<>(Arrays.asList(
	'                "bert/embeddings/one_hot/on_value",
	'                "bert/embeddings/one_hot/off_value",
	'                "bert/embeddings/LayerNorm/batchnorm/add/y",    //Scalar - Eps Constant?
	'                "bert/embeddings/dropout/keep_prob",
	'                "bert/encoder/ones",
	'                "bert/embeddings/dropout/random_uniform/min",   //Dropout scalar values
	'                "bert/embeddings/dropout/random_uniform/max"
	'
	'        ));

			Dim floatConstants As ISet(Of String) = New HashSet(Of String)(java.util.Arrays.asList("bert/encoder/ones"))

			'For training, convert weights and biases from constants to variables:
			For Each v As SDVariable In sd.variables()
				If v.Constant AndAlso v.dataType().isFPType() AndAlso Not v.Arr.Scalar AndAlso Not floatConstants.Contains(v.name()) Then 'Skip scalars - trainable params
					log.info("Converting to variable: {} - dtype: {} - shape: {}", v.name(), v.dataType(), java.util.Arrays.toString(v.Arr.shape()))
					v.convertToVariable()
				End If
			Next v

			Console.WriteLine("INPUTS: " & sd.inputs())
			Console.WriteLine("OUTPUTS: " & sd.outputs())

			'For training, we'll need to add a label placeholder for one-hot labels:
			Dim label As SDVariable = sd.placeHolder("label", DataType.FLOAT, 4, 2)
			Dim softmax As SDVariable = sd.getVariable("loss/Softmax")
			sd.loss().logLoss("loss", label, softmax)
			assertEquals(Collections.singletonList("loss"), sd.getLossVariables())

			'Peform simple overfitting test - same input, but inverted labels

			Dim ex1Idxs As INDArray = Nd4j.createFromArray(101,1996,12368,3115,1004,3532,1005,1055,3156,5950,1026,1012,11867,2595,1028,2001,1014,1012,4805,2685,2896,1010,2030,1014,1012,5709,3867,1010,2012,5585,2581,1012,6185,1012,102,1996,2974,1011,17958,17235,2850,4160,12490,5950,1012,11814,2594,2001,2039,1021,1012,4413,2685,1010,2030,1014,1012,3429,3867,1010,2012,1015,1010,3515,2509,1012,4008,1012,102,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0)
			Dim ex1Mask As INDArray = Nd4j.createFromArray(1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0)
			Dim ex1SegmentId As INDArray = Nd4j.createFromArray(0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0)

			Dim ex2Idxs As INDArray = Nd4j.createFromArray(101,6661,1999,8670,2020,2091,1015,1012,1019,3867,2012,16923,7279,3401,2011,16087,2692,13938,2102,1010,2125,1037,2659,1997,17943,2361,1010,1999,1037,3621,6428,3452,2414,3006,1012,102,6661,1999,8670,2020,2091,2093,3867,2012,13913,1011,1015,1013,1018,7279,3401,2011,5641,22394,13938,2102,1010,2125,1037,2659,1997,17943,7279,3401,1010,1999,1037,6428,3006,1012,102,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0)
			Dim ex2Mask As INDArray = Nd4j.createFromArray(1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0)
			Dim ex2SegmentId As INDArray = Nd4j.createFromArray(0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0)

			Dim ex3Idxs As INDArray = Nd4j.createFromArray(101,2197,2095,1010,4012,10526,2772,1015,1012,1019,2454,2047,3617,5830,17073,1012,102,4012,10526,2038,2055,2538,1012,1017,2454,5830,17073,1010,2116,1999,1996,2922,1057,1012,1055,1012,3655,1012,102,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0)
			Dim ex3Mask As INDArray = Nd4j.createFromArray(1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0)
			Dim ex3SegmentId As INDArray = Nd4j.createFromArray(0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0)

			Dim ex4Idxs As INDArray = Nd4j.createFromArray(101,6599,3123,1017,1012,1023,3867,1010,2000,1002,1015,1012,6191,4551,2013,1002,1015,1012,5401,4551,1012,102,1996,17602,1010,3448,1011,2241,2194,2056,3780,6599,3445,1019,3867,2000,1002,1015,1012,4805,4551,1012,102,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0)
			Dim ex4Mask As INDArray = Nd4j.createFromArray(1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0)
			Dim ex4SegmentId As INDArray = Nd4j.createFromArray(0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0)

			Dim idxs As INDArray = Nd4j.vstack(ex1Idxs, ex2Idxs, ex3Idxs, ex4Idxs)
			Dim mask As INDArray = Nd4j.vstack(ex1Mask, ex2Mask, ex3Mask, ex4Mask)
			Dim segmentIdxs As INDArray = Nd4j.vstack(ex1SegmentId, ex2SegmentId, ex3SegmentId, ex4SegmentId)
			Dim labelArr As INDArray = Nd4j.createFromArray(New Single()(){
				New Single() {1, 0},
				New Single() {0, 1},
				New Single() {1, 0},
				New Single() {1, 0}
			})

			Dim c As TrainingConfig = TrainingConfig.builder().updater(New Adam(2e-5)).l2(1e-5).dataSetFeatureMapping("IteratorGetNext", "IteratorGetNext:1", "IteratorGetNext:4").dataSetLabelMapping("label").build()
			sd.TrainingConfig = c

			Dim mds As MultiDataSet = New org.nd4j.linalg.dataset.MultiDataSet(New INDArray(){idxs, mask, segmentIdxs}, New INDArray(){labelArr})

			Dim placeholderValues As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			placeholderValues("IteratorGetNext") = idxs
			placeholderValues("IteratorGetNext:1") = mask
			placeholderValues("IteratorGetNext:4") = segmentIdxs
			placeholderValues("label") = labelArr

			Dim lossArr As INDArray = sd.output(placeholderValues, "loss")("loss")
			assertTrue(lossArr.Scalar)
			Dim scoreBefore As Double = lossArr.getDouble(0)
			For i As Integer = 0 To 4
				sd.fit(mds)
			Next i

			lossArr = sd.output(placeholderValues, "loss")("loss")
			assertTrue(lossArr.Scalar)
			Dim scoreAfter As Double = lossArr.getDouble(0)

			Dim s As String = "Before: " & scoreBefore & "; after: " & scoreAfter
			assertTrue(scoreAfter < scoreBefore,s)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void writeBertUI(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub writeBertUI(ByVal backend As Nd4jBackend)
			'Test used to generate graph for visualization to work out appropriate subgraph structure to replace
			Dim f As New File("C:/Temp/TF_Graphs/mrpc_output/frozen/bert_mrpc_frozen.pb")
			Dim minibatchSize As Integer = 4

			Dim m As IDictionary(Of String, TFImportOverride) = New Dictionary(Of String, TFImportOverride)()
			m("IteratorGetNext") = Function(inputs, controlDepInputs, nodeDef, initWith, attributesForNode, graph)
			Return java.util.Arrays.asList(initWith.placeHolder("IteratorGetNext", DataType.INT, minibatchSize, 128), initWith.placeHolder("IteratorGetNext:1", DataType.INT, minibatchSize, 128), initWith.placeHolder("IteratorGetNext:4", DataType.INT, minibatchSize, 128))
			End Function

			'Skip the "IteratorV2" op - we don't want or need this
			Dim filter As TFOpImportFilter = Function(nodeDef, initWith, attributesForNode, graph)
			Return "IteratorV2".Equals(nodeDef.getName())
			End Function

			Dim sd As SameDiff = TFGraphMapper.importGraph(f, m, filter)

			Dim w As New LogFileWriter(New File("C:/Temp/BERT_UI.bin"))
			Dim bytesWritten As Long = w.writeGraphStructure(sd)
			Dim bytesWritten2 As Long = w.writeFinishStaticMarker()
		End Sub

	End Class

End Namespace