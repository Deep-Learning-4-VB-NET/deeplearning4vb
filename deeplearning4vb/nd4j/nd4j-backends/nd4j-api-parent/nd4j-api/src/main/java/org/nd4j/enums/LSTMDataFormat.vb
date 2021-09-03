﻿''' <summary>
'''*****************************************************************************
''' Copyright (c) 2019-2020 Konduit K.K.
''' 
''' This program and the accompanying materials are made available under the
''' terms of the Apache License, Version 2.0 which is available at
''' https://www.apache.org/licenses/LICENSE-2.0.
''' 
''' Unless required by applicable law or agreed to in writing, software
''' distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
''' WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
''' License for the specific language governing permissions and limitations
''' under the License.
''' 
''' SPDX-License-Identifier: Apache-2.0
''' *****************************************************************************
''' </summary>

'================== GENERATED CODE - DO NOT MODIFY THIS FILE ==================

Namespace org.nd4j.enums

	''' <summary>
	''' for unidirectional:  TNS: shape [timeLength, numExamples, inOutSize] - sometimes referred to as "time major"<br>
	'''   NST: shape [numExamples, inOutSize, timeLength]<br>
	'''   NTS: shape [numExamples, timeLength, inOutSize] - TF "time_major=false" layout<br> for bidirectional:
	'''    T2NS: 3 = [timeLength, 2, numExamples, inOutSize] (for ONNX) 
	''' </summary>
	Public Enum LSTMDataFormat
	  TNS

	  NST

	  NTS

	  T2NS
	End Enum

End Namespace