﻿Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports AfterEach = org.junit.jupiter.api.AfterEach
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports TFGraphMapper = org.nd4j.imports.graphmapper.tf.TFGraphMapper
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ClassPathResource = org.nd4j.common.io.ClassPathResource
Imports NativeOpsHolder = org.nd4j.nativeblas.NativeOpsHolder
import static org.junit.jupiter.api.Assertions.assertEquals

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

Namespace org.nd4j.imports


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class ExecutionTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class ExecutionTests
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @AfterEach public void tearDown()
		Public Overridable Sub tearDown()
			NativeOpsHolder.Instance.getDeviceNativeOps().enableDebugMode(False)
			NativeOpsHolder.Instance.getDeviceNativeOps().enableVerboseMode(False)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testStoredGraph_1() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testStoredGraph_1()
			Nd4j.create(1)

			Dim tg As val = TFGraphMapper.importGraphTxt((New ClassPathResource("tf_graphs/reduce_dim.pb.txt")).InputStream, Nothing, Nothing)
	'        System.out.println(tg.summary());

			Dim result_0 As IDictionary(Of String, INDArray) = tg.outputAll(Nothing)
			Dim exp_0 As val = Nd4j.create(DataType.FLOAT, 3).assign(3.0)

			assertEquals(exp_0, result_0("Sum"))
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace