Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports DeepLearning4VB.org.nd4vb.linalg.managed
Imports DeepLearning4VB.org.nd4vb.linalg.managed.ops
Imports DeepLearning4VB.org.nd4vb.linalg.api.ops
Imports DifferentialFunction = DeepLearning4VB.org.nd4vb.autodiff.functions.DifferentialFunction
Imports BaseRandomOp = DeepLearning4VB.org.nd4vb.linalg.api.ops.random.BaseRandomOp
Imports OpType = DeepLearning4VB.org.nd4vb.linalg.api.ops.Op.Type

Namespace org.nd4vb.linalg.api.ops.impl

    ''' <summary>
    ''' Table-driven reflection tests over the custom op classes of phase 2 wave 2b (the <c>DynamicCustomOp</c>
    ''' subclasses in <c>ops.impl</c>, <c>ops.custom</c>, <c>ops.compat</c>, <c>ops.compression</c>,
    ''' <c>ops.persistence</c>, <c>ops.util</c> and <c>ops.random.custom</c>/<c>compat</c>, and the legacy random ops in
    ''' <c>ops.random.impl</c>). The op names in the table were taken from the Java classes (the converter text in
    ''' git), not from the port: every class instantiates, keeps its Java <c>opName()</c>, the names are unique, and
    ''' every custom op whose math the managed registry knows has a class.
    ''' </summary>
    <TestClass>
    Public Class CustomOpTableTests

        Private Const OpsNamespace As String = "DeepLearning4VB.org.nd4vb.linalg.api.ops."

        ''' <summary>Class (relative to <c>...linalg.api.ops</c>) and its Java <c>opName()</c> (with the parameterless constructor).</summary>
        Private Shared ReadOnly Table As String(,) = {
            {"compat.CompatSparseToDense", "compat_sparse_to_dense"},
            {"compat.CompatStringSplit", "compat_string_split"},
            {"compression.DecodeBitmap", "decode_bitmap"},
            {"compression.DecodeThreshold", "decode_threshold"},
            {"compression.EncodeBitmap", "encode_bitmap"},
            {"compression.EncodeThreshold", "encode_threshold"},
            {"custom.AdjustContrast", "adjust_contrast_v2"},
            {"custom.AdjustHue", "adjust_hue"},
            {"custom.AdjustSaturation", "adjust_saturation"},
            {"custom.BarnesEdgeForces", "barnes_edge_forces"},
            {"custom.BarnesHutGains", "barnes_gains"},
            {"custom.BarnesHutSymmetrize", "barnes_symmetrized"},
            {"custom.BetaInc", "betainc"},
            {"custom.BitCast", "bitcast"},
            {"custom.CompareAndBitpack", "compare_and_bitpack"},
            {"custom.Digamma", "digamma"},
            {"custom.DivideNoNan", "divide_no_nan"},
            {"custom.DrawBoundingBoxes", "draw_bounding_boxes"},
            {"custom.FakeQuantWithMinMaxVarsPerChannel", "fake_quant_with_min_max_vars_per_channel"},
            {"custom.Flatten", "flatten"},
            {"custom.FusedBatchNorm", "fused_batch_norm"},
            {"custom.HsvToRgb", "hsv_to_rgb"},
            {"custom.Igamma", "igamma"},
            {"custom.Igammac", "igammac"},
            {"custom.KnnMinDistance", "knn_mindistance"},
            {"custom.Lgamma", "lgamma"},
            {"custom.LinearSolve", "solve"},
            {"custom.Logdet", "logdet"},
            {"custom.Lstsq", "lstsq"},
            {"custom.Lu", "lu"},
            {"custom.MatrixBandPart", "matrix_band_part"},
            {"custom.Polygamma", "polygamma"},
            {"custom.RandomCrop", "random_crop"},
            {"custom.RgbToGrayscale", "rgb_to_grs"},
            {"custom.RgbToHsv", "rgb_to_hsv"},
            {"custom.RgbToYiq", "rgb_to_yiq"},
            {"custom.RgbToYuv", "rgb_to_yuv"},
            {"custom.Roll", "roll"},
            {"custom.SpTreeCell", "cell_contains"},
            {"custom.ToggleBits", "toggle_bits"},
            {"custom.Tri", "tri"},
            {"custom.TriangularSolve", "triangular_solve"},
            {"custom.Triu", "triu"},
            {"custom.TriuBp", "triu_bp"},
            {"custom.YiqToRgb", "yiq_to_rgb"},
            {"custom.YuvToRgb", "yuv_to_rgb"},
            {"impl.broadcast.BiasAdd", "biasadd"},
            {"impl.broadcast.BiasAddGrad", "biasadd_bp"},
            {"impl.broadcast.BroadcastTo", "broadcast_to"},
            {"impl.controlflow.compat.Enter", "enter"},
            {"impl.controlflow.compat.Exit", "exit"},
            {"impl.controlflow.compat.LoopCond", "loop_cond"},
            {"impl.controlflow.compat.Merge", "merge"},
            {"impl.controlflow.compat.NextIteration", "next_iteration"},
            {"impl.controlflow.compat.StopGradient", "stop_gradient"},
            {"impl.controlflow.compat.Switch", "switch"},
            {"impl.controlflow.compat.While", "While"},
            {"impl.controlflow.Select", "select"},
            {"impl.controlflow.Where", "Where"},
            {"impl.controlflow.WhereNumpy", "where_np"},
            {"impl.image.CropAndResize", "crop_and_resize"},
            {"impl.image.ExtractImagePatches", "extract_image_patches"},
            {"impl.image.ImageResize", "image_resize"},
            {"impl.image.NonMaxSuppression", "non_max_suppression"},
            {"impl.image.NonMaxSuppressionV3", "non_max_suppression_v3"},
            {"impl.image.NonMaxSuppressionWithOverlaps", "non_max_suppression_overlaps"},
            {"impl.image.ResizeArea", "resize_area"},
            {"impl.image.ResizeBicubic", "resize_bicubic"},
            {"impl.image.ResizeBilinear", "resize_bilinear"},
            {"impl.image.ResizeNearestNeighbor", "resize_nearest_neighbor"},
            {"impl.indexaccum.custom.ArgAmax", "argamax"},
            {"impl.indexaccum.custom.ArgAmin", "argamin"},
            {"impl.indexaccum.custom.ArgMax", "argmax"},
            {"impl.indexaccum.custom.ArgMin", "argmin"},
            {"impl.layers.convolution.AvgPooling2D", "avgpool2d"},
            {"impl.layers.convolution.AvgPooling3D", "avgpool3dnew"},
            {"impl.layers.convolution.BatchNorm", "batchnorm"},
            {"impl.layers.convolution.BatchNormDerivative", "batchnorm_bp"},
            {"impl.layers.convolution.Col2Im", "col2im"},
            {"impl.layers.convolution.Conv1D", "conv1d"},
            {"impl.layers.convolution.Conv1DDerivative", "conv1d_bp"},
            {"impl.layers.convolution.Conv2D", "conv2d"},
            {"impl.layers.convolution.Conv2DDerivative", "conv2d_bp"},
            {"impl.layers.convolution.Conv3D", "conv3dnew"},
            {"impl.layers.convolution.Conv3DDerivative", "conv3dnew_bp"},
            {"impl.layers.convolution.DeConv2D", "deconv2d"},
            {"impl.layers.convolution.DeConv2DDerivative", "deconv2d_bp"},
            {"impl.layers.convolution.DeConv2DTF", "deconv2d_tf"},
            {"impl.layers.convolution.DeConv3D", "deconv3d"},
            {"impl.layers.convolution.DeConv3DDerivative", "deconv3d_bp"},
            {"impl.layers.convolution.DeConv3DTF", "deconv3d_tf"},
            {"impl.layers.convolution.DepthToSpace", "depth_to_space"},
            {"impl.layers.convolution.DepthwiseConv2D", "depthwise_conv2d"},
            {"impl.layers.convolution.DepthwiseConv2DBp", "depthwise_conv2d_bp"},
            {"impl.layers.convolution.Im2col", "im2col"},
            {"impl.layers.convolution.Im2colBp", "im2col_bp"},
            {"impl.layers.convolution.LocalResponseNormalization", "lrn"},
            {"impl.layers.convolution.LocalResponseNormalizationDerivative", "lrn_bp"},
            {"impl.layers.convolution.MaxPooling2D", "maxpool2d"},
            {"impl.layers.convolution.MaxPooling3D", "maxpool3dnew"},
            {"impl.layers.convolution.MaxPoolWithArgmax", "max_pool_with_argmax"},
            {"impl.layers.convolution.Pooling2D", "somepoolingpool2d"},
            {"impl.layers.convolution.Pooling2DDerivative", "somepoolingpool2d_bp"},
            {"impl.layers.convolution.Pooling3DDerivative", "pooling3dpool3dnew_bp"},
            {"impl.layers.convolution.SConv2D", "sconv2d"},
            {"impl.layers.convolution.SConv2DDerivative", "sconv2d_bp"},
            {"impl.layers.convolution.SpaceToDepth", "space_to_depth"},
            {"impl.layers.convolution.Upsampling2d", "upsampling2d"},
            {"impl.layers.convolution.Upsampling2dDerivative", "upsampling2d_bp"},
            {"impl.layers.convolution.Upsampling3d", "upsampling3d"},
            {"impl.layers.convolution.Upsampling3dBp", "upsampling3d_bp"},
            {"impl.layers.ExternalErrorsFunction", "ExternalErrorsFn"},
            {"impl.layers.recurrent.GRU", "gru"},
            {"impl.layers.recurrent.GRUBp", "gru_bp"},
            {"impl.layers.recurrent.GRUCell", "gruCell"},
            {"impl.layers.recurrent.LSTMBlock", "lstmBlock"},
            {"impl.layers.recurrent.LSTMBlockCell", "lstmBlockCell"},
            {"impl.layers.recurrent.LSTMCell", "lstmCell"},
            {"impl.layers.recurrent.LSTMLayer", "lstmLayer"},
            {"impl.layers.recurrent.LSTMLayerBp", "lstmLayer_bp"},
            {"impl.layers.recurrent.SRU", "sru"},
            {"impl.layers.recurrent.SRUCell", "sruCell"},
            {"impl.loss.AbsoluteDifferenceLoss", "absolute_difference_loss"},
            {"impl.loss.bp.AbsoluteDifferenceLossBp", "absolute_difference_loss_grad"},
            {"impl.loss.bp.CosineDistanceLossBp", "cosine_distance_loss_grad"},
            {"impl.loss.bp.CtcLossBp", "ctc_loss_grad"},
            {"impl.loss.bp.HingeLossBp", "hinge_loss_grad"},
            {"impl.loss.bp.HuberLossBp", "huber_loss_grad"},
            {"impl.loss.bp.LogLossBp", "log_loss_grad"},
            {"impl.loss.bp.LogPoissonLossBp", "log_poisson_loss_grad"},
            {"impl.loss.bp.MeanPairwiseSquaredErrorLossBp", "mean_pairwssqerr_loss_grad"},
            {"impl.loss.bp.MeanSquaredErrorLossBp", "mean_sqerr_loss_grad"},
            {"impl.loss.bp.SigmoidCrossEntropyLossBp", "sigm_cross_entropy_loss_grad"},
            {"impl.loss.bp.SoftmaxCrossEntropyLossBp", "softmax_cross_entropy_loss_grad"},
            {"impl.loss.bp.SoftmaxCrossEntropyWithLogitsLossBp", "softmax_cross_entropy_loss_with_logits_grad"},
            {"impl.loss.bp.SparseSoftmaxCrossEntropyLossWithLogitsBp", "sparse_softmax_cross_entropy_loss_with_logits_grad"},
            {"impl.loss.CosineDistanceLoss", "cosine_distance_loss"},
            {"impl.loss.CtcLoss", "ctc_loss"},
            {"impl.loss.HingeLoss", "hinge_loss"},
            {"impl.loss.HuberLoss", "huber_loss"},
            {"impl.loss.L2Loss", "l2_loss"},
            {"impl.loss.LogLoss", "log_loss"},
            {"impl.loss.LogPoissonLoss", "log_poisson_loss"},
            {"impl.loss.MeanPairwiseSquaredErrorLoss", "mean_pairwssqerr_loss"},
            {"impl.loss.MeanSquaredErrorLoss", "mean_sqerr_loss"},
            {"impl.loss.SigmoidCrossEntropyLoss", "sigm_cross_entropy_loss"},
            {"impl.loss.SoftmaxCrossEntropyLoss", "softmax_cross_entropy_loss"},
            {"impl.loss.SoftmaxCrossEntropyWithLogitsLoss", "softmax_cross_entropy_loss_with_logits"},
            {"impl.loss.SparseSoftmaxCrossEntropyLossWithLogits", "sparse_softmax_cross_entropy_loss_with_logits"},
            {"impl.loss.WeightedCrossEntropyLoss", "weighted_cross_entropy_with_logits"},
            {"impl.nlp.CbowRound", "cbow"},
            {"impl.nlp.SkipGramRound", "skipgram"},
            {"impl.reduce.bp.CumProdBp", "cumprod_bp"},
            {"impl.reduce.bp.CumSumBp", "cumsum_bp"},
            {"impl.reduce.bp.DotBp", "reduce_dot_bp"},
            {"impl.reduce.bp.MaxBp", "reduce_max_bp"},
            {"impl.reduce.bp.MeanBp", "reduce_mean_bp"},
            {"impl.reduce.bp.MinBp", "reduce_min_bp"},
            {"impl.reduce.bp.Norm1Bp", "reduce_norm1_bp"},
            {"impl.reduce.bp.Norm2Bp", "reduce_norm2_bp"},
            {"impl.reduce.bp.NormMaxBp", "reduce_norm_max_bp"},
            {"impl.reduce.bp.PowBp", "Pow_bp"},
            {"impl.reduce.bp.ProdBp", "reduce_prod_bp"},
            {"impl.reduce.bp.SquaredNormBp", "reduce_sqnorm_bp"},
            {"impl.reduce.bp.StandardDeviationBp", "reduce_stdev_bp"},
            {"impl.reduce.bp.SumBp", "reduce_sum_bp"},
            {"impl.reduce.bp.VarianceBp", "reduce_variance_bp"},
            {"impl.reduce.custom.BatchMmul", "batched_gemm"},
            {"impl.reduce.custom.LogSumExp", "reduce_logsumexp"},
            {"impl.reduce.HashCode", "hashcode"},
            {"impl.reduce.Mmul", "matmul"},
            {"impl.reduce.MmulBp", "matmul_bp"},
            {"impl.reduce.Moments", "moments"},
            {"impl.reduce.NormalizeMoments", "normalize_moments"},
            {"impl.reduce.SufficientStatistics", "sufficient_statistics"},
            {"impl.reduce.TensorMmul", "tensordot"},
            {"impl.reduce.TensorMmulBp", "tensormmul_bp"},
            {"impl.reduce.ZeroFraction", "zero_fraction"},
            {"impl.scalar.PRelu", "prelu"},
            {"impl.scalar.RectifiedLinearDerivative", "relu_bp"},
            {"impl.scatter.ScatterAdd", "scatter_add"},
            {"impl.scatter.ScatterDiv", "scatter_div"},
            {"impl.scatter.ScatterMax", "scatter_max"},
            {"impl.scatter.ScatterMin", "scatter_min"},
            {"impl.scatter.ScatterMul", "scatter_mul"},
            {"impl.scatter.ScatterNd", "scatter_nd"},
            {"impl.scatter.ScatterNdAdd", "scatter_nd_add"},
            {"impl.scatter.ScatterNdSub", "scatter_nd_sub"},
            {"impl.scatter.ScatterNdUpdate", "scatter_nd_update"},
            {"impl.scatter.ScatterSub", "scatter_sub"},
            {"impl.scatter.ScatterUpdate", "scatter_update"},
            {"impl.shape.ApplyGradientDescent", "applygradientdescent"},
            {"impl.shape.bp.ConcatBp", "concat_bp"},
            {"impl.shape.bp.MergeAvgBp", "mergeavg_bp"},
            {"impl.shape.bp.MergeMaxBp", "mergemax_bp"},
            {"impl.shape.bp.SliceBp", "slice_bp"},
            {"impl.shape.bp.StridedSliceBp", "strided_slice_bp"},
            {"impl.shape.bp.TileBp", "tile_bp"},
            {"impl.shape.BroadcastDynamicShape", "broadcast_dynamic_shape"},
            {"impl.shape.Concat", "concat"},
            {"impl.shape.ConfusionMatrix", "confusion_matrix"},
            {"impl.shape.Create", "create"},
            {"impl.shape.Cross", "cross"},
            {"impl.shape.Diag", "diag"},
            {"impl.shape.DiagPart", "diag_part"},
            {"impl.shape.ExpandDims", "expand_dims"},
            {"impl.shape.Eye", "eye"},
            {"impl.shape.Flatten2D", "flatten_2d"},
            {"impl.shape.Gather", "gather"},
            {"impl.shape.GatherNd", "gather_nd"},
            {"impl.shape.Linspace", "lin_space"},
            {"impl.shape.MergeAvg", "mergeavg"},
            {"impl.shape.MergeMax", "mergemax"},
            {"impl.shape.MergeMaxIndex", "mergemaxindex"},
            {"impl.shape.MergeSum", "mergesum"},
            {"impl.shape.MeshGrid", "meshgrid"},
            {"impl.shape.OneHot", "onehot"},
            {"impl.shape.OnesLike", "ones_as"},
            {"impl.shape.ParallelStack", "parallel_stack"},
            {"impl.shape.Permute", "permute"},
            {"impl.shape.Rank", "rank"},
            {"impl.shape.ReductionShape", "evaluate_reduction_shape"},
            {"impl.shape.Repeat", "repeat"},
            {"impl.shape.Reshape", "reshape"},
            {"impl.shape.SequenceMask", "sequence_mask"},
            {"impl.shape.Shape", "shape_of"},
            {"impl.shape.ShapeN", "shapes_of"},
            {"impl.shape.Size", "size"},
            {"impl.shape.SizeAt", "size_at"},
            {"impl.shape.Slice", "slice"},
            {"impl.shape.Split", "split"},
            {"impl.shape.SplitV", "split_v"},
            {"impl.shape.Squeeze", "squeeze"},
            {"impl.shape.Stack", "stack"},
            {"impl.shape.StridedSlice", "strided_slice"},
            {"impl.shape.tensorops.EmbeddingLookup", "embedding_lookup"},
            {"impl.shape.tensorops.TensorArray", "tensorarrayv3"},
            {"impl.shape.tensorops.TensorArrayConcat", "stack_list"},
            {"impl.shape.tensorops.TensorArrayGather", "gather_list"},
            {"impl.shape.tensorops.TensorArrayRead", "read_list"},
            {"impl.shape.tensorops.TensorArrayScatter", "scatter_list"},
            {"impl.shape.tensorops.TensorArraySize", "size_list"},
            {"impl.shape.tensorops.TensorArraySplit", "split_list"},
            {"impl.shape.tensorops.TensorArrayWrite", "tensorarraywritev3"},
            {"impl.shape.Tile", "tile"},
            {"impl.shape.Transpose", "transpose"},
            {"impl.shape.Unstack", "unstack"},
            {"impl.shape.ZerosLike", "zeroslike"},
            {"impl.transforms.Angle", "zeros_like"},
            {"impl.transforms.any.IsMax", "ismax"},
            {"impl.transforms.Assert", "Assert"},
            {"impl.transforms.BinCount", "bincount"},
            {"impl.transforms.CheckNumerics", "check_numerics"},
            {"impl.transforms.Cholesky", "cholesky"},
            {"impl.transforms.clip.ClipByAvgNorm", "clipbyavgnorm"},
            {"impl.transforms.clip.ClipByNorm", "clipbynorm"},
            {"impl.transforms.clip.ClipByNormBp", "clipbynorm_bp"},
            {"impl.transforms.clip.ClipByValue", "ClipByValue"},
            {"impl.transforms.custom.Assign", "assign"},
            {"impl.transforms.custom.ATan2", "tf_atan2"},
            {"impl.transforms.custom.BatchToSpace", "batch_to_space"},
            {"impl.transforms.custom.BatchToSpaceND", "batch_to_space_nd"},
            {"impl.transforms.custom.BitsHammingDistance", "bits_hamming_distance"},
            {"impl.transforms.custom.BitwiseAnd", "bitwise_and"},
            {"impl.transforms.custom.BitwiseOr", "bitwise_or"},
            {"impl.transforms.custom.BitwiseXor", "bitwise_xor"},
            {"impl.transforms.custom.Choose", "choose"},
            {"impl.transforms.custom.CReLU", "crelu"},
            {"impl.transforms.custom.CReluBp", "crelu_bp"},
            {"impl.transforms.custom.CumProd", "cumprod"},
            {"impl.transforms.custom.CumSum", "cumsum"},
            {"impl.transforms.custom.CyclicRShiftBits", "cyclic_rshift_bits"},
            {"impl.transforms.custom.CyclicShiftBits", "cyclic_shift_bits"},
            {"impl.transforms.custom.Dilation2D", "dilation2d"},
            {"impl.transforms.custom.DotProductAttention", "dot_product_attention"},
            {"impl.transforms.custom.DotProductAttentionBp", "dot_product_attention_bp"},
            {"impl.transforms.custom.DynamicPartition", "dynamic_partition"},
            {"impl.transforms.custom.DynamicStitch", "dynamic_stitch"},
            {"impl.transforms.custom.EqualTo", "equals"},
            {"impl.transforms.custom.FakeQuantWithMinMaxArgs", "fake_quant_with_min_max_args"},
            {"impl.transforms.custom.FakeQuantWithMinMaxVars", "fake_quant_with_min_max_vars"},
            {"impl.transforms.custom.Fill", "fill"},
            {"impl.transforms.custom.GreaterThan", "greater"},
            {"impl.transforms.custom.GreaterThanOrEqual", "greater_equal"},
            {"impl.transforms.custom.InTopK", "in_top_k"},
            {"impl.transforms.custom.InvertPermutation", "invert_permutation"},
            {"impl.transforms.custom.IsNonDecreasing", "is_non_decreasing"},
            {"impl.transforms.custom.IsNumericTensor", "is_numeric_tensor"},
            {"impl.transforms.custom.IsStrictlyIncreasing", "is_strictly_increasing"},
            {"impl.transforms.custom.LayerNorm", "layer_norm"},
            {"impl.transforms.custom.LayerNormBp", "layer_norm_bp"},
            {"impl.transforms.custom.LessThan", "less"},
            {"impl.transforms.custom.LessThanOrEqual", "less_equal"},
            {"impl.transforms.custom.ListDiff", "listdiff"},
            {"impl.transforms.custom.LogicalAnd", "boolean_and"},
            {"impl.transforms.custom.LogicalNot", "boolean_not"},
            {"impl.transforms.custom.LogicalOr", "boolean_or"},
            {"impl.transforms.custom.LogicalXor", "boolean_xor"},
            {"impl.transforms.custom.LogMatrixDeterminant", "log_matrix_determinant"},
            {"impl.transforms.custom.LogSoftMax", "log_softmax"},
            {"impl.transforms.custom.MatrixDeterminant", "matrix_determinant"},
            {"impl.transforms.custom.MatrixDiag", "matrix_diag"},
            {"impl.transforms.custom.MatrixDiagPart", "matrix_diag_part"},
            {"impl.transforms.custom.MatrixInverse", "matrix_inverse"},
            {"impl.transforms.custom.MatrixSetDiag", "matrix_set_diag"},
            {"impl.transforms.custom.Max", "maximum"},
            {"impl.transforms.custom.MaximumBp", "maximum_bp"},
            {"impl.transforms.custom.Min", "minimum"},
            {"impl.transforms.custom.MirrorPad", "mirror_pad"},
            {"impl.transforms.custom.MultiHeadDotProductAttention", "multi_head_dot_product_attention"},
            {"impl.transforms.custom.MultiHeadDotProductAttentionBp", "multi_head_dot_product_attention_bp"},
            {"impl.transforms.custom.NotEqualTo", "not_equals"},
            {"impl.transforms.custom.ParallelConcat", "ParallelConcat"},
            {"impl.transforms.custom.Pow", "Pow"},
            {"impl.transforms.custom.Qr", "qr"},
            {"impl.transforms.custom.Reverse", "reverse"},
            {"impl.transforms.custom.ReverseBp", "reverse_bp"},
            {"impl.transforms.custom.ReverseSequence", "reverse_sequence"},
            {"impl.transforms.custom.ReverseV2", "reverse_v2"},
            {"impl.transforms.custom.RShiftBits", "rshift_bits"},
            {"impl.transforms.custom.segment.SegmentMax", "segment_max"},
            {"impl.transforms.custom.segment.SegmentMean", "segment_mean"},
            {"impl.transforms.custom.segment.SegmentMin", "segment_min"},
            {"impl.transforms.custom.segment.SegmentProd", "segment_prod"},
            {"impl.transforms.custom.segment.SegmentSum", "segment_sum"},
            {"impl.transforms.custom.ShiftBits", "shift_bits"},
            {"impl.transforms.custom.SoftMax", "softmax"},
            {"impl.transforms.custom.SpaceToBatch", "space_to_batch"},
            {"impl.transforms.custom.SpaceToBatchND", "space_to_batch_nd"},
            {"impl.transforms.custom.Standardize", "standardize"},
            {"impl.transforms.custom.StandardizeBp", "standardize_bp"},
            {"impl.transforms.custom.Svd", "svd"},
            {"impl.transforms.custom.ThresholdRelu", "thresholdedrelu"},
            {"impl.transforms.custom.TopK", "top_k"},
            {"impl.transforms.custom.Trace", "trace"},
            {"impl.transforms.custom.Unique", "unique"},
            {"impl.transforms.custom.UniqueWithCounts", "unique_with_counts"},
            {"impl.transforms.custom.XwPlusB", "xw_plus_b"},
            {"impl.transforms.custom.Zeta", "zeta"},
            {"impl.transforms.dtype.Cast", "cast"},
            {"impl.transforms.gradient.CubeBp", "cube_bp"},
            {"impl.transforms.gradient.DynamicPartitionBp", "dynamic_partition_bp"},
            {"impl.transforms.gradient.EluBp", "elu_bp"},
            {"impl.transforms.gradient.GradientBackwardsMarker", "gradientbackwards"},
            {"impl.transforms.gradient.HardSigmoidBp", "hardsigmoid_bp"},
            {"impl.transforms.gradient.HardTanhBp", "hardtanh_bp"},
            {"impl.transforms.gradient.LeakyReLUBp", "lrelu_bp"},
            {"impl.transforms.gradient.LogSoftMaxDerivative", "log_softmax_bp"},
            {"impl.transforms.gradient.PReluBp", "prelu_bp"},
            {"impl.transforms.gradient.RationalTanhBp", "rationaltanh_bp"},
            {"impl.transforms.gradient.RectifiedTanhBp", "rectifiedtanh_bp"},
            {"impl.transforms.gradient.Relu6Derivative", "relu6_bp"},
            {"impl.transforms.gradient.SeluBp", "selu_bp"},
            {"impl.transforms.gradient.SigmoidDerivative", "sigmoid_bp"},
            {"impl.transforms.gradient.SoftmaxBp", "softmax_bp"},
            {"impl.transforms.gradient.SoftPlusBp", "softplus_bp"},
            {"impl.transforms.gradient.SoftSignBp", "softsign_bp"},
            {"impl.transforms.gradient.TanhDerivative", "tanh_bp"},
            {"impl.transforms.gradient.ThresholdReluBp", "thresholdedrelu_bp"},
            {"impl.transforms.Histogram", "histogram"},
            {"impl.transforms.HistogramFixedWidth", "histogram_fixed_width"},
            {"impl.transforms.IdentityN", "identity_n"},
            {"impl.transforms.NthElement", "nth_element"},
            {"impl.transforms.Pad", "pad"},
            {"impl.transforms.pairwise.arithmetic.AddOp", "add"},
            {"impl.transforms.pairwise.arithmetic.bp.AddBpOp", "add_bp"},
            {"impl.transforms.pairwise.arithmetic.bp.DivBpOp", "divide_bp"},
            {"impl.transforms.pairwise.arithmetic.bp.FloorDivBpOp", "floordiv_bp"},
            {"impl.transforms.pairwise.arithmetic.bp.FloorModBpOp", "floormod_bp"},
            {"impl.transforms.pairwise.arithmetic.bp.MergeAddBp", "mergeadd_bp"},
            {"impl.transforms.pairwise.arithmetic.bp.ModBpOp", "mod_bp"},
            {"impl.transforms.pairwise.arithmetic.bp.MulBpOp", "multiply_bp"},
            {"impl.transforms.pairwise.arithmetic.bp.RDivBpOp", "reversedivide_bp"},
            {"impl.transforms.pairwise.arithmetic.bp.RSubBpOp", "reversesubtract_bp"},
            {"impl.transforms.pairwise.arithmetic.bp.SquaredDifferenceBpOp", "squaredsubtract_bp"},
            {"impl.transforms.pairwise.arithmetic.bp.SubBpOp", "subtract_bp"},
            {"impl.transforms.pairwise.arithmetic.DivOp", "divide"},
            {"impl.transforms.pairwise.arithmetic.FloorDivOp", "floordiv"},
            {"impl.transforms.pairwise.arithmetic.FloorModOp", "floormod"},
            {"impl.transforms.pairwise.arithmetic.MergeAddOp", "mergeadd"},
            {"impl.transforms.pairwise.arithmetic.ModOp", "mod"},
            {"impl.transforms.pairwise.arithmetic.MulOp", "multiply"},
            {"impl.transforms.pairwise.arithmetic.RDivOp", "reversedivide"},
            {"impl.transforms.pairwise.arithmetic.RealDivOp", "realdiv"},
            {"impl.transforms.pairwise.arithmetic.RSubOp", "reversesubtract"},
            {"impl.transforms.pairwise.arithmetic.SquaredDifferenceOp", "squaredsubtract"},
            {"impl.transforms.pairwise.arithmetic.SubOp", "subtract"},
            {"impl.transforms.pairwise.arithmetic.TruncateDivOp", "truncatediv"},
            {"impl.transforms.ReluLayer", "relu_layer"},
            {"impl.transforms.same.Identity", "identity"},
            {"impl.transforms.segment.bp.SegmentMaxBp", "segment_max_bp"},
            {"impl.transforms.segment.bp.SegmentMeanBp", "segment_mean_bp"},
            {"impl.transforms.segment.bp.SegmentMinBp", "segment_min_bp"},
            {"impl.transforms.segment.bp.SegmentProdBp", "segment_prod_bp"},
            {"impl.transforms.segment.bp.SegmentSumBp", "segment_sum_bp"},
            {"impl.transforms.segment.bp.UnsortedSegmentMaxBp", "unsorted_segment_max_bp"},
            {"impl.transforms.segment.bp.UnsortedSegmentMeanBp", "unsorted_segment_mean_bp"},
            {"impl.transforms.segment.bp.UnsortedSegmentMinBp", "unsorted_segment_min_bp"},
            {"impl.transforms.segment.bp.UnsortedSegmentProdBp", "unsorted_segment_prod_bp"},
            {"impl.transforms.segment.bp.UnsortedSegmentSqrtNBp", "unsorted_segment_sqrt_n_bp"},
            {"impl.transforms.segment.bp.UnsortedSegmentSumBp", "unsorted_segment_sum_bp"},
            {"impl.transforms.segment.UnsortedSegmentMax", "unsorted_segment_max"},
            {"impl.transforms.segment.UnsortedSegmentMean", "unsorted_segment_mean"},
            {"impl.transforms.segment.UnsortedSegmentMin", "unsorted_segment_min"},
            {"impl.transforms.segment.UnsortedSegmentProd", "unsorted_segment_prod"},
            {"impl.transforms.segment.UnsortedSegmentSqrtN", "unsorted_segment_sqrt_n"},
            {"impl.transforms.segment.UnsortedSegmentSum", "unsorted_segment_sum"},
            {"impl.transforms.strict.ELU", "elu"},
            {"impl.updaters.AdaBeliefUpdater", "adabelief_updater"},
            {"impl.updaters.AdaDeltaUpdater", "ada_delta_updater"},
            {"impl.updaters.AdaGradUpdater", "ada_grad_updater"},
            {"impl.updaters.AdaMaxUpdater", "ada_max_updater"},
            {"impl.updaters.AdamUpdater", "adam_updater"},
            {"impl.updaters.AmsGradUpdater", "ams_grad_updater"},
            {"impl.updaters.NadamUpdater", "nadam_updater"},
            {"impl.updaters.NesterovsUpdater", "nesterovs_updater"},
            {"impl.updaters.RmsPropUpdater", "rms_prop_updater"},
            {"impl.updaters.SgdUpdater", "sgd_updater"},
            {"persistence.RestoreV2", "restorev2"},
            {"persistence.SaveV2", "savev2"},
            {"random.compat.RandomStandardNormal", "random_normal"},
            {"random.custom.DistributionUniform", "randomuniform"},
            {"random.custom.RandomBernoulli", "random_bernoulli"},
            {"random.custom.RandomExponential", "random_exponential"},
            {"random.custom.RandomGamma", "random_gamma"},
            {"random.custom.RandomNormal", "randomnormal"},
            {"random.custom.RandomPoisson", "random_poisson"},
            {"random.custom.RandomShuffle", "random_shuffle"},
            {"random.impl.AlphaDropOut", "alpha_dropout"},
            {"random.impl.BernoulliDistribution", "distribution_bernoulli"},
            {"random.impl.BinomialDistribution", "distribution_binomial"},
            {"random.impl.BinomialDistributionEx", "distribution_binomial_ex"},
            {"random.impl.Choice", "choice"},
            {"random.impl.DropOut", "dropout"},
            {"random.impl.DropOutInverted", "dropout_inverted"},
            {"random.impl.GaussianDistribution", "distribution_gaussian"},
            {"random.impl.Linspace", "linspace_random"},
            {"random.impl.LogNormalDistribution", "distribution_lognormal"},
            {"random.impl.ProbablisticMerge", "probablistic_merge"},
            {"random.impl.Range", "range"},
            {"random.impl.TruncatedNormalDistribution", "distribution_truncated"},
            {"random.impl.UniformDistribution", "distribution_uniform"},
            {"util.PrintAffinity", "print_affinity"},
            {"util.PrintVariable", "print_variable"}
        }

        ''' <summary>Java classes without a public parameterless constructor (Lombok's @NoArgsConstructor is absent).</summary>
        Private Shared ReadOnly WithoutParameterlessConstructor As String() = Array.Empty(Of String)()

        ''' <summary>
        ''' Classes whose parameterless constructor needs a wave-3 stand-in: the legacy random ops initialize their
        ''' data type field with <c>Nd4vb.defaultFloatingPointType()</c> (Java's <c>BaseRandomOp</c>).
        ''' </summary>
        Private Shared ReadOnly NeedStandIn As String() = {
            "random.impl.AlphaDropOut",
            "random.impl.BernoulliDistribution",
            "random.impl.BinomialDistribution",
            "random.impl.BinomialDistributionEx",
            "random.impl.Choice",
            "random.impl.DropOut",
            "random.impl.DropOutInverted",
            "random.impl.GaussianDistribution",
            "random.impl.Linspace",
            "random.impl.LogNormalDistribution",
            "random.impl.ProbablisticMerge",
            "random.impl.TruncatedNormalDistribution",
            "random.impl.UniformDistribution"
        }

        Private Shared ReadOnly PortedNamespaces As String() = {"impl.", "custom.", "compat.", "compression.", "persistence.", "util.", "random."}

        Private Shared Function Rows() As IEnumerable(Of Integer)
            Return Enumerable.Range(0, Table.GetLength(0))
        End Function

        Private Shared Function TypeOfRow(ByVal row As Integer) As Type
            Dim t As Type = GetType(DynamicCustomOp).Assembly.GetType(OpsNamespace & Table(row, 0), False)
            assertTrue(t IsNot Nothing, Table(row, 0))
            Return t
        End Function

        ''' <summary>An instance: from the parameterless constructor when there is one, otherwise uninitialized.</summary>
        Private Shared Function Instance(ByVal row As Integer) As DifferentialFunction
            Dim t As Type = TypeOfRow(row)
            If t.GetConstructor(Type.EmptyTypes) Is Nothing Then
#If NETFRAMEWORK Then
                Return DirectCast(System.Runtime.Serialization.FormatterServices.GetUninitializedObject(t), DifferentialFunction)
#Else
                Return DirectCast(System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(t), DifferentialFunction)
#End If
            End If
            Return DirectCast(CreateOp(t, Nothing), DifferentialFunction)
        End Function

        ''' <summary>The concrete op classes of wave 2b: custom ops in the ported packages, legacy random ops in ops.random.</summary>
        Private Shared Function PortedOpClasses() As List(Of String)
            Return GetType(DynamicCustomOp).Assembly.GetTypes().
                Where(Function(t) t.IsClass AndAlso Not t.IsAbstract AndAlso GetType(DifferentialFunction).IsAssignableFrom(t) AndAlso
                          t.FullName.StartsWith(OpsNamespace, StringComparison.Ordinal) AndAlso
                          PortedNamespaces.Any(Function(p) t.FullName.Substring(OpsNamespace.Length).StartsWith(p, StringComparison.Ordinal)) AndAlso
                          (GetType(DynamicCustomOp).IsAssignableFrom(t) OrElse GetType(BaseRandomOp).IsAssignableFrom(t))).
                Select(Function(t) t.FullName.Substring(OpsNamespace.Length)).
                OrderBy(Function(s) s, StringComparer.Ordinal).ToList()
        End Function

        <TestMethod>
        Public Sub TheTableListsEveryPortedOpClass()
            Dim listed As List(Of String) = Rows().Select(Function(r) Table(r, 0)).OrderBy(Function(s) s, StringComparer.Ordinal).ToList()
            assertEquals(444, listed.Count)
            assertEquals(listed, PortedOpClasses())
        End Sub

        <TestMethod>
        Public Sub EveryOpWithAPublicParameterlessConstructorInstantiates()
            Dim noDefault As New List(Of String)()
            Dim standIn As New List(Of String)()
            For Each row As Integer In Rows()
                Dim t As Type = TypeOfRow(row)
                If t.GetConstructor(Type.EmptyTypes) Is Nothing Then
                    noDefault.Add(Table(row, 0))
                    Continue For
                End If
                Dim constructed As Boolean
                Dim op As Object = CreateOp(t, constructed)
                assertTrue(TypeOf op Is DifferentialFunction, Table(row, 0))
                If Not constructed Then
                    standIn.Add(Table(row, 0))
                End If
            Next
            assertEquals(WithoutParameterlessConstructor.OrderBy(Function(s) s, StringComparer.Ordinal).ToList(), noDefault.OrderBy(Function(s) s, StringComparer.Ordinal).ToList())
            ' with the real Nd4vb (wave 3) none needs a stand-in any more
            assertTrue(standIn.All(Function(s) NeedStandIn.Contains(s)), "unexpected stand-in constructions: " & String.Join(", ", standIn))
        End Sub

        <TestMethod>
        Public Sub EveryOpKeepsItsJavaName()
            For Each row As Integer In Rows()
                Dim op As DifferentialFunction = Instance(row)
                Dim name As String = op.opName()
                assertFalse(String.IsNullOrEmpty(name), Table(row, 0))
                assertEquals(Table(row, 1), name, Table(row, 0))
            Next
        End Sub

        <TestMethod>
        Public Sub OpNamesAreUnique()
            ' Java shares no op name between these classes, nor with the 173 legacy op classes
            Dim names As List(Of String) = Rows().Select(Function(r) Table(r, 1)).ToList()
            Dim dups As List(Of String) = names.GroupBy(Function(n) n).Where(Function(g) g.Count() > 1).Select(Function(g) g.Key).ToList()
            assertEquals(0, dups.Count, "shared names: " & String.Join(", ", dups))
            Dim legacy As New HashSet(Of String)(StringComparer.Ordinal)
            For Each t As Type In GetType(BaseOp).Assembly.GetTypes()
                If t.IsClass AndAlso Not t.IsAbstract AndAlso GetType(BaseOp).IsAssignableFrom(t) AndAlso
                   t.FullName.StartsWith(OpsNamespace & "impl.", StringComparison.Ordinal) Then
                    legacy.Add(DirectCast(CreateOp(t, Nothing), BaseOp).opName())
                End If
            Next
            assertEquals(173, legacy.Count)
            Dim both As List(Of String) = names.Where(Function(n) legacy.Contains(n)).ToList()
            assertEquals(0, both.Count, "custom and legacy ops sharing a name: " & String.Join(", ", both))
        End Sub

        <TestMethod>
        Public Sub EveryCustomOpWithRegisteredMathHasAClass()
            Dim names As New HashSet(Of String)(Rows().Select(Function(r) Table(r, 1)), StringComparer.Ordinal)
            Dim registered As String() = ManagedOpMath.Names(ManagedOpKind.Custom)
            assertTrue(registered.Length >= 52, "registered custom names: " & registered.Length)
            For Each n As String In registered
                Dim family As ManagedOpKind
                assertTrue(ManagedOpMath.TryGetCustomFamily(n, family), n)
                assertTrue(names.Contains(n), "no op class named " & n)
            Next
        End Sub

        <TestMethod>
        Public Sub OpTypesFollowTheBaseClasses()
            Dim counts As New Dictionary(Of OpType, Integer)()
            For Each row As Integer In Rows()
                Dim op As DifferentialFunction = Instance(row)
                Dim type As OpType = op.opType()
                If TypeOf op Is BaseRandomOp Then
                    assertEquals(OpType.RANDOM, type, Table(row, 0))
                ElseIf type <> OpType.CUSTOM Then
                    ' the TF control-flow ops (and SameDiff's external gradients) report their own types, as in Java
                    assertTrue(Table(row, 0).StartsWith("impl.controlflow.compat.", StringComparison.Ordinal) OrElse
                               Table(row, 0) = "impl.layers.ExternalErrorsFunction", Table(row, 0) & ": " & type.ToString())
                End If
                Dim n As Integer = 0
                counts.TryGetValue(type, n)
                counts(type) = n + 1
            Next
            assertEquals(13, counts(OpType.RANDOM))
            assertEquals(7, counts(OpType.LOGIC))
            assertEquals(1, counts(OpType.LOOP_COND))
            assertEquals(444 - 13 - 8, counts(OpType.CUSTOM))
            assertEquals(4, counts.Count)
        End Sub

    End Class

End Namespace
