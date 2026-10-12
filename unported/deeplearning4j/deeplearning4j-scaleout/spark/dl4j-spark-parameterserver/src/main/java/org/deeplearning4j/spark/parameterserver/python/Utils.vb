Imports JavaRDD = org.apache.spark.api.java.JavaRDD
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports DataSet = org.nd4j.linalg.dataset.DataSet

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

Namespace org.deeplearning4j.spark.parameterserver.python


	Public Class Utils

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static ArrayDescriptor getArrayDescriptor(org.nd4j.linalg.api.ndarray.INDArray arr) throws Exception
		Private Shared Function getArrayDescriptor(ByVal arr As INDArray) As ArrayDescriptor
			Return New ArrayDescriptor(arr)
		End Function

		Private Shared Function getArray(ByVal arrDesc As ArrayDescriptor) As INDArray
			Return arrDesc.Array
		End Function

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: private static DataSetDescriptor getDataSetDescriptor(org.nd4j.linalg.dataset.DataSet ds)throws Exception
		Private Shared Function getDataSetDescriptor(ByVal ds As DataSet) As DataSetDescriptor
			Return New DataSetDescriptor(ds)
		End Function

		Private Shared Function getDataSet(ByVal dsDesc As DataSetDescriptor) As DataSet
			Return dsDesc.DataSet
		End Function
		Public Shared Function getArrayDescriptorRDD(ByVal indarrayRDD As JavaRDD(Of INDArray)) As JavaRDD(Of ArrayDescriptor)
			Return indarrayRDD.map(AddressOf Utils.getArrayDescriptor)
		End Function

		Public Shared Function getArrayRDD(ByVal arrayDescriptorRDD As JavaRDD(Of ArrayDescriptor)) As JavaRDD(Of INDArray)
'JAVA TO VB CONVERTER TODO TASK: Method reference arbitrary object instance method syntax is not converted by Java to VB Converter:
			Return arrayDescriptorRDD.map(ArrayDescriptor::getArray)
		End Function

		Public Shared Function getDatasetDescriptorRDD(ByVal dsRDD As JavaRDD(Of DataSet)) As JavaRDD(Of DataSetDescriptor)
			Return dsRDD.map(AddressOf Utils.getDataSetDescriptor)
		End Function

		Public Shared Function getDataSetRDD(ByVal dsDescriptorRDD As JavaRDD(Of DataSetDescriptor)) As JavaRDD(Of DataSet)
			Return dsDescriptorRDD.map(AddressOf Utils.getDataSet)
		End Function
	End Class

End Namespace