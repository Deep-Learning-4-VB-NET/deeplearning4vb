Imports System.Collections.Generic
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports AttrValue = org.tensorflow.framework.AttrValue
Imports GraphDef = org.tensorflow.framework.GraphDef
Imports NodeDef = org.tensorflow.framework.NodeDef

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

Namespace org.nd4j.linalg.api.ops.persistence


	Public Class RestoreV2
		Inherits DynamicCustomOp


		Public Overrides Function opName() As String
			Return "restorev2"
		End Function


		Public Overrides Function onnxName() As String
			Throw New NoOpNameFoundException("No op found for saving.")
		End Function

		Public Overrides Function tensorflowName() As String
			Return "RestoreV2"
		End Function


		Public Overrides Sub initFromTensorFlow(ByVal nodeDef As NodeDef, ByVal initWith As SameDiff, ByVal attributesForNode As IDictionary(Of String, AttrValue), ByVal graph As GraphDef)
	'         
	'            strided slice typically takes 4 tensor arguments:
	'            0) input, it's shape determines number of elements in other arguments
	'            1) begin indices
	'            2) end indices
	'            3) strides
	'         

	'        val inputBegin = tNode.getInputs().get(1);
	'        val inputEnd = tNode.getInputs().get(2);
	'        val inputStrides = tNode.getInputs().get(3);
	'
	'
	'        val iArgs = new ArrayList<Integer>();
	'
	'        // bit masks for this slice
	'        val bm = nodeDef.getAttrOrThrow("begin_mask");
	'        val xm = nodeDef.getAttrOrThrow("ellipsis_mask");
	'        val em = nodeDef.getAttrOrThrow("end_mask");
	'        val nm = nodeDef.getAttrOrThrow("new_axis_mask");
	'        val sm = nodeDef.getAttrOrThrow("shrink_axis_mask");
	'
	'        iArgs.add((int) bm.getI());
	'        iArgs.add((int) xm.getI());
	'        iArgs.add((int) em.getI());
	'
	'        iArgs.add((int) nm.getI());
	'        iArgs.add((int) sm.getI());
	'
	'        if (inputBegin.getNode() < 0 && inputEnd.getNode() < 0 && inputStrides.getNode() < 0) {
	'
	'            // order matters, hehe
	'            val strides = graph.getVariableSpace().getVariable(tNode.getInputs().remove(3));
	'            val end = graph.getVariableSpace().getVariable(tNode.getInputs().remove(2));
	'            val begin = graph.getVariableSpace().getVariable(tNode.getInputs().remove(1));
	'
	'            for (int e = 0; e < begin.getArray().length(); e++)
	'                iArgs.add((int) begin.getArray().getInt(e));
	'
	'            for (int e = 0; e < end.getArray().length(); e++)
	'                iArgs.add((int) end.getArray().getInt(e));
	'
	'            for (int e = 0; e < strides.getArray().length(); e++)
	'                iArgs.add((int) strides.getArray().getInt(e));
	'        } else {
	'            // do nothing
	'        }
	'
	'        val bits = Ints.toArray(iArgs);
		End Sub



	End Class

End Namespace