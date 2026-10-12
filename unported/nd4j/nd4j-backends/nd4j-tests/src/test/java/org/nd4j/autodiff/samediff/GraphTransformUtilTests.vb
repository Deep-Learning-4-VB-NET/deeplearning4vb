Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports GraphTransformUtil = org.nd4j.autodiff.samediff.transform.GraphTransformUtil
Imports OpPredicate = org.nd4j.autodiff.samediff.transform.OpPredicate
Imports SubGraph = org.nd4j.autodiff.samediff.transform.SubGraph
Imports SubGraphPredicate = org.nd4j.autodiff.samediff.transform.SubGraphPredicate
Imports SubGraphProcessor = org.nd4j.autodiff.samediff.transform.SubGraphProcessor
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports AddOp = org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.AddOp
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
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

Namespace org.nd4j.autodiff.samediff


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Tag(TagNames.SAMEDIFF) public class GraphTransformUtilTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class GraphTransformUtilTests
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBasic(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testBasic(ByVal backend As Nd4jBackend)

			Dim sd As SameDiff = SameDiff.create()
			Dim ph1 As SDVariable = sd.placeHolder("ph1", DataType.FLOAT, -1, 32)
			Dim ph2 As SDVariable = sd.placeHolder("ph2", DataType.FLOAT, -1, 32)

			Dim add As SDVariable = ph1.add(ph2)
			Dim add2 As SDVariable = add.add(ph1)

			Dim [sub] As SDVariable = add.sub(add2)

			assertTrue(OpPredicate.classEquals(GetType(AddOp)).matches(sd, sd.getVariableOutputOp(add.name())))
			assertTrue(OpPredicate.classEquals(GetType(AddOp)).matches(sd, sd.getVariableOutputOp(add2.name())))
			assertFalse(OpPredicate.classEquals(GetType(AddOp)).matches(sd, sd.getVariableOutputOp([sub].name())))

			assertTrue(OpPredicate.opNameEquals(AddOp.OP_NAME).matches(sd, sd.getVariableOutputOp(add.name())))
			assertTrue(OpPredicate.opNameEquals(AddOp.OP_NAME).matches(sd, sd.getVariableOutputOp(add2.name())))
			assertFalse(OpPredicate.opNameEquals(AddOp.OP_NAME).matches(sd, sd.getVariableOutputOp([sub].name())))

			assertTrue(OpPredicate.opNameMatches(".*dd").matches(sd, sd.getVariableOutputOp(add.name())))
			assertTrue(OpPredicate.opNameMatches("ad.*").matches(sd, sd.getVariableOutputOp(add2.name())))
			assertFalse(OpPredicate.opNameMatches(".*dd").matches(sd, sd.getVariableOutputOp([sub].name())))


			Dim p As SubGraphPredicate = SubGraphPredicate.withRoot(OpPredicate.classEquals(GetType(AddOp)))

			Dim l As IList(Of SubGraph) = GraphTransformUtil.getSubgraphsMatching(sd, p)
			assertEquals(2, l.Count)

			Dim sg1 As SubGraph = l(0)
			assertTrue(sg1.getRootNode() Is sd.getVariableOutputOp(add.name()))
			assertEquals(0, sg1.getChildNodes().size())

			Dim sg2 As SubGraph = l(1)
			assertTrue(sg2.getRootNode() Is sd.getVariableOutputOp(add2.name()))
			assertEquals(0, sg2.getChildNodes().size())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSubgraphReplace1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSubgraphReplace1(ByVal backend As Nd4jBackend)

			Dim sd As SameDiff = SameDiff.create()
			Dim ph1 As SDVariable = sd.placeHolder("ph1", DataType.FLOAT, -1, 4)
			Dim ph2 As SDVariable = sd.placeHolder("ph2", DataType.FLOAT, -1, 4)

			Dim p1 As INDArray = Nd4j.ones(DataType.FLOAT, 1, 4)
			Dim p2 As INDArray = Nd4j.ones(DataType.FLOAT, 1, 4).mul(3)
			ph1.Array = p1
			ph2.Array = p2

			Dim add As SDVariable = ph1.add(ph2)
			Dim [sub] As SDVariable = ph1.sub(ph2)
			Dim mul As SDVariable = add.mul([sub])

	'        INDArray out = mul.eval();
	'        INDArray exp = p1.add(p2).mul(p1.sub(p2));
	'        assertEquals(exp, out);

			Dim p As SubGraphPredicate = SubGraphPredicate.withRoot(OpPredicate.classEquals(GetType(AddOp)))

			Dim sd2 As SameDiff = GraphTransformUtil.replaceSubgraphsMatching(sd, p, New SubGraphProcessorAnonymousInnerClass(Me))

			Dim exp2 As INDArray = p1.div(p2).mul(p1.sub(p2))
			Dim out2 As INDArray = sd2.getVariable(mul.name()).eval()
			assertEquals(exp2, out2)


		End Sub

		Private Class SubGraphProcessorAnonymousInnerClass
			Implements SubGraphProcessor

			Private ReadOnly outerInstance As GraphTransformUtilTests

			Public Sub New(ByVal outerInstance As GraphTransformUtilTests)
				Me.outerInstance = outerInstance
			End Sub

			Public Function processSubgraph(ByVal sd As SameDiff, ByVal subGraph As SubGraph) As IList(Of SDVariable)
				'Let's replace add op with div op
				assertTrue(subGraph.getChildNodes() Is Nothing OrElse subGraph.getChildNodes().isEmpty())
				Dim inputs As IList(Of SDVariable) = subGraph.inputs()
				Dim [out] As SDVariable = inputs(0).div(inputs(1))
				Return Collections.singletonList([out])
			End Function
		End Class

	End Class

End Namespace